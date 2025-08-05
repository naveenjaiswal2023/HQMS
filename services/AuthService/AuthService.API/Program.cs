using AuthService.API.Middleware;
using AuthService.Application;
using AuthService.Application.Commands;
using AuthService.Domain.Identity;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SharedInfrastructure.Settings;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Add Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// ✅ Load configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var configuration = builder.Configuration;
var environment = builder.Environment;

// ✅ Build DB connection
var connTemplate = configuration.GetConnectionString("AuthDbConnectionString")
    ?? Environment.GetEnvironmentVariable("AuthDbConnectionString");

var dbPassword = configuration["QmsDbPassword"]
    ?? Environment.GetEnvironmentVariable("QmsDbPassword");

if (string.IsNullOrEmpty(connTemplate))
    throw new InvalidOperationException("AuthDbConnectionString is missing.");

if (string.IsNullOrEmpty(dbPassword))
    throw new InvalidOperationException("QmsDbPassword is missing.");

var actualConnectionString = connTemplate.Replace("_QmsDbPassword_", dbPassword);
Console.WriteLine($"[ENV: {environment.EnvironmentName}] Connection String SET: {(!string.IsNullOrWhiteSpace(actualConnectionString)).ToString()}");

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlServer(actualConnectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null);
    });
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

// ✅ JWT Setup
var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key))
    throw new InvalidOperationException("JWT settings are missing or invalid.");

builder.Services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(jwtSettings.Key);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            if (environment.IsDevelopment())
            {
                Console.WriteLine($"JWT Auth failed: {context.Exception.Message}");
            }
            return Task.CompletedTask;
        }
    };
});

// ✅ Register services AFTER DbContext & Identity
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(configuration);

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

// ✅ CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{

    {
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter 'Bearer' followed by your token."
    });

    {
            new OpenApiSecurityScheme
        {
            {
            },
            Array.Empty<string>()
        }
    });
    // 👇 Add server URL to Swagger for API Gateway compatibility
    c.AddServer(new OpenApiServer
    {
        Url = "https://localhost:7260/auth" // Match your YARP route
});
});

builder.Services.AddControllers();

var app = builder.Build();

// ✅ Middleware
if (app.Environment.IsDevelopment())
{
    // 👇 Rewrites Swagger server path to support API Gateway (/auth/)
    //app.UseSwagger(c =>
    //{
    //    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    //    {
    //        var pathBase = "/auth"; // 👈 match the prefix used in API Gateway
    //        swaggerDoc.Servers = new List<OpenApiServer>
    //        {
    //            new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{pathBase}" }
    //        };
    //    });
    //});

    //app.UseSwaggerUI(c =>
    //{
    //    // 👇 Change the route prefix and endpoint path
    //    c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "Auth Service API V1");
    //    c.RoutePrefix = "swagger"; // Swagger UI available at /swagger
    //});

    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
    });

    // ✅ Seed roles and admin user
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = ["Admin", "Doctor", "Patient", "POD"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new ApplicationRole { Name = role });
        }

        var adminEmail = "admin@hqms.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Admin",
                Gender = "Male",
                DateOfBirth = new DateTime(1990, 1, 1),
                PhoneNumber = "1234567890",
                IsActive = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123"); // Secure password
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                foreach (var error in result.Errors)
                    Console.WriteLine($"Error creating user: {error.Description}");
            }
        }
    }

}
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
