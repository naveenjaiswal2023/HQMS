using AppointmentService.API.Middleware;
using AppointmentService.Application;
using AppointmentService.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Add Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// ✅ Load Configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var config = builder.Configuration;
var env = builder.Environment;

// ✅ Build secure DB connection string
var connTemplate = config.GetConnectionString("AppointmentDbConnectionString")
    ?? Environment.GetEnvironmentVariable("AppointmentDbConnectionString");

var dbPassword = config["QmsDbPassword"]
    ?? Environment.GetEnvironmentVariable("QmsDbPassword");

if (string.IsNullOrWhiteSpace(connTemplate))
    throw new InvalidOperationException("Connection string missing.");

if (string.IsNullOrWhiteSpace(dbPassword))
    throw new InvalidOperationException("QmsDbPassword missing.");

var actualConnectionString = connTemplate.Replace("_QmsDbPassword_", dbPassword);
Console.WriteLine($"[ENV: {env.EnvironmentName}] Connection String SET: {(!string.IsNullOrWhiteSpace(actualConnectionString))}");

// ✅ JWT Authentication Setup
var jwtKey = config["JwtSettings:Key"];
if (string.IsNullOrEmpty(jwtKey))
    throw new InvalidOperationException("JWT secret key is missing in configuration.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = config["JwtSettings:Issuer"],

        ValidateAudience = true,
        ValidAudience = config["JwtSettings:Audience"],

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,

        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var claims = context.Principal?.Claims;
            var hasRole = claims?.Any(c => c.Type == ClaimTypes.Role) == true;
            var hasClientId = claims?.Any(c => c.Type == "client_id") == true;

            if (!hasRole && !hasClientId)
                context.Fail("Token does not contain required claims (role or client_id).");

            return Task.CompletedTask;
        }
    };
});

// ✅ Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(ClaimTypes.Role);
    });

    options.AddPolicy("InternalPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("client_id");
    });
});

builder.Services.AddHttpContextAccessor();

// ✅ Application & Infrastructure
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(config, actualConnectionString);

// ✅ CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ✅ Swagger with JWT + Gateway-aware rewrite
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Appointment Service API",
        Version = "v1"
    });
    options.AddServer(new OpenApiServer { Url = "/appointments" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: `Bearer eyJ...`",
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

// ✅ Controllers
builder.Services.AddControllers();

// ✅ Build and run app
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/appointments/swagger/v1/swagger.json", "Appointment Service API V1");
        c.RoutePrefix = "swagger"; // Available at /appointments/swagger/
    });

}
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
