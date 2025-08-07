using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QueuehService.API.Middleware;
using QueueService.Application;
using QueueService.Infrastructure;
using Serilog;
using SharedInfrastructure.Settings;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ Serilog configuration
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// ✅ Load configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var config = builder.Configuration;
var env = builder.Environment;

// ✅ Get connection string from environment variable
var actualConnectionString = Environment.GetEnvironmentVariable("QueueDbConnectionString");
if (string.IsNullOrWhiteSpace(actualConnectionString))
    throw new InvalidOperationException("QueueDbConnectionString environment variable is missing.");

Console.WriteLine($"[ENV: {env.EnvironmentName}] QueueDbConnectionString Loaded: true");

// ✅ Register HttpContextAccessor and Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(config, actualConnectionString);

// ✅ Bind strongly typed JwtSettings
var jwtSettingsSection = config.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);
var jwtSettings = jwtSettingsSection.Get<JwtSettings>();

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
    throw new InvalidOperationException("JWT secret key is missing in configuration.");

// ✅ JWT Authentication Configuration
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

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var claims = context.Principal?.Claims;
            var hasRole = claims?.Any(c => c.Type == ClaimTypes.Role) == true;
            var hasClientId = claims?.Any(c => c.Type == "client_id") == true;

            if (!hasRole && !hasClientId)
            {
                context.Fail("Token does not contain required claims (role or client_id).");
            }

            return Task.CompletedTask;
        }
    };
});

// ✅ Authorization Policies
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

// ✅ CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ✅ Swagger Configuration with Bearer Token Support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Queue Service API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.  
Enter your token below. Example: `Bearer eyJhbGciOi...`",
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

    options.AddServer(new OpenApiServer
    {
        Url = "https://localhost:7260/queue"
    });
});

// ✅ Add Controllers
builder.Services.AddControllers();

// ✅ Build and Configure the App
var app = builder.Build();

// ✅ Swagger and Exception Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/queue/swagger/v1/swagger.json", "Queue API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
