using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PatientService.Application.Interfaces;
using PatientService.Application.Services;
using PaymentService.API.Middleware;
using PaymentService.Application.Commands;
using PaymentService.Application.Common.Behaviours;
using PaymentService.Application.Interfaces;
using PaymentService.Application.Validators;
using PaymentService.Domain.Events;
using PaymentService.Domain.Models.Payments;
using PaymentService.Infrastructure;
using PaymentService.Infrastructure.Models;
using PaymentService.Infrastructure.Persistence;
using PaymentService.Infrastructure.Services;
using Serilog;
using SharedInfrastructure.Settings;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ------------------- Serilog -------------------
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
// ------------------- Load Configuration -------------------
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var config = builder.Configuration;
var env = builder.Environment;

// Database
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentDbConnectionString")));

// Patient Service Client
builder.Services.AddHttpClient<IPatientRegistrationService, PatientRegistrationService>((sp, client) =>
{
    var baseUrl = builder.Configuration.GetValue<string>("PatientService:BaseUrl");
    if (string.IsNullOrEmpty(baseUrl))
        throw new InvalidOperationException("PatientService:BaseUrl is missing in configuration.");

    client.BaseAddress = new Uri(baseUrl);
});

// Register configurations
builder.Services.Configure<RazorpayConfig>(builder.Configuration.GetSection("RazorpayConfig"));
builder.Services.Configure<StripeConfig>(builder.Configuration.GetSection("StripeConfig"));

// MediatR with behaviors
//builder.Services.AddMediatR(cfg =>
//{
//    cfg.RegisterServicesFromAssembly(typeof(InitiateRegistrationPaymentCommand).Assembly);
//    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
//    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
//    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
//});
builder.Services.AddMediatR(cfg =>
{
    // register from Application and Domain layers
    cfg.RegisterServicesFromAssembly(typeof(InitiateRegistrationPaymentCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(PaymentCompletedEvent).Assembly);
});
// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(InitiateRegistrationPaymentCommandValidator).Assembly);

// ------------------- JWT Settings -------------------
var jwtSettingsSection = config.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);
var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
if (string.IsNullOrWhiteSpace(jwtSettings.Key))
    throw new InvalidOperationException("JWT secret key is missing in configuration.");

// ------------------- Authentication -------------------
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
            ClockSkew = TimeSpan.Zero,
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
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ------------------- Swagger -------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Payment Service API",
        Version = "v1"
    });

    // ✅ Add the Gateway base path so Swagger knows to prepend "/patients"
    options.AddServer(new OpenApiServer { Url = "/payments" });

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
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new List<string>()
        }
    });
});

// Health Checks
//builder.Services.AddHealthChecks()
//    .AddCheck<DatabaseHealthCheck>("database");

// API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Configuration.GetConnectionString("PaymentDbConnectionString"));
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

// ------------------- Middleware -------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Inside the service, it should be local path
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
//app.MapHealthChecks("/health");

app.Run();