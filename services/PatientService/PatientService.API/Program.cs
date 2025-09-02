using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PatientService.API.Middleware;
using PatientService.Application.Commands;
using PatientService.Application.Common.Behaviours;
using PatientService.Application.Interfaces;
using PatientService.Application.Services;
using PatientService.Application.Validators;
using PatientService.Domain.Interfaces;
using PatientService.Infrastructure;
using PatientService.Infrastructure.Persistence;
using Serilog;
using SharedInfrastructure.Settings;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ------------------- Serilog Logging -------------------
builder.Host.UseSerilog((context, configuration) =>
{
    // Read all Serilog configuration from appsettings.json
    configuration.ReadFrom.Configuration(context.Configuration);
});

// ------------------- Configuration -------------------
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// ------------------- Database -------------------
builder.Services.AddDbContext<PatientDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PatientDbConnectionString"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
            sqlOptions.CommandTimeout(60);
        }));

// ------------------- MediatR & Pipeline Behaviors -------------------
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterPatientWithPaymentCommand).Assembly));

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

// ------------------- FluentValidation -------------------
builder.Services.AddValidatorsFromAssembly(typeof(RegisterPatientWithPaymentCommandValidator).Assembly);

// ------------------- HTTP Clients -------------------
builder.Services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["PaymentService:BaseUrl"]);
    client.Timeout = TimeSpan.FromSeconds(30); // Production-friendly timeout
});

// ------------------- JWT Authentication -------------------
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);
var jwtSettings = jwtSettingsSection.Get<JwtSettings>()
                  ?? throw new InvalidOperationException("JWT settings not found");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role
        };
    });

// ------------------- Authorization -------------------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy =>
        policy.RequireAuthenticatedUser().RequireClaim(ClaimTypes.Role));

    options.AddPolicy("InternalPolicy", policy =>
        policy.RequireAuthenticatedUser().RequireClaim("client_id"));
});

// ------------------- CORS -------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        policy.WithOrigins(allowedOrigins ?? Array.Empty<string>())
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ------------------- Swagger -------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Patient Service API",
        Version = "v1"
    });

    // ✅ Add the Gateway base path so Swagger knows to prepend "/patients"
    options.AddServer(new OpenApiServer { Url = "/patients" });

    // 🔒 JWT Auth support
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

// ------------------- Health Checks -------------------
//builder.Services.AddHealthChecks()
//    .AddSqlServer<PatientDbContext>(builder.Configuration.GetConnectionString("PatientDbConnectionString"))
//    .AddUrlGroup(
//        new Uri(builder.Configuration["PaymentService:BaseUrl"]!),
//        name: "PaymentService",
//        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded
//    );

// ------------------- Dependency Injection -------------------
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

// ------------------- Controllers -------------------
builder.Services.AddControllers();

var app = builder.Build();

// ------------------- Middleware Pipeline -------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Inside the service, it should be local path
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Patient API V1");
        c.RoutePrefix = "swagger";
    });
}


app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Global Exception Handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Health Check Endpoint with JSON response
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration.ToString()
            }),
            totalDuration = report.TotalDuration.ToString()
        };

        await context.Response.WriteAsJsonAsync(result);
    }
});

app.MapControllers();
app.Run();
