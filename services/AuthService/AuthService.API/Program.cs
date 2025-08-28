using AuthService.API.Middleware;
using AuthService.Application;
using AuthService.Domain.Identity;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SharedInfrastructure.Settings;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Logging
builder.Host.UseSerilog((ctx, config) =>
    config.ReadFrom.Configuration(ctx.Configuration));

// 🔹 Configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var configuration = builder.Configuration;
var environment = builder.Environment;

// 🔹 Database Connection
var connectionString = configuration.GetConnectionString("AuthDbConnectionString")
    ?? Environment.GetEnvironmentVariable("AuthDbConnectionString")
    ?? throw new InvalidOperationException("AuthDbConnectionString is missing.");

Log.Information("[{Env}] Connection string available: {HasConn}", environment.EnvironmentName, !string.IsNullOrWhiteSpace(connectionString));

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null)));

// 🔹 Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

// 🔹 JWT
var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings section missing.");

builder.Services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

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
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var accessToken = ctx.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken) && ctx.HttpContext.Request.Path.StartsWithSegments("/notificationHub"))
                {
                    ctx.Token = accessToken;
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = ctx =>
            {
                if (environment.IsDevelopment())
                    Log.Warning("JWT authentication failed: {Message}", ctx.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });

// 🔹 DI
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(configuration);
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

// 🔹 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(configuration["AllowedFrontend"] ?? "http://localhost:60424")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 🔹 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth Service API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
    });

    c.AddServer(new OpenApiServer { Url = "https://localhost:7260/auth" }); // for Gateway compatibility
});

builder.Services.AddControllers();

var app = builder.Build();

// 🔹 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Service API V1");
        c.RoutePrefix = "swagger";
    });

    await SeedData.EnsureSeedDataAsync(app);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();


// 🔹 Seed Data Helper
static class SeedData
{
    public static async Task EnsureSeedDataAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

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

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(adminUser, "Admin");
            else
                foreach (var error in result.Errors)
                    Log.Error("Error creating admin user: {Error}", error.Description);
        }
    }
}
