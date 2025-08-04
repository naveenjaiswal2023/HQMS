using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QueueService.Application;
using QueueService.Infrastructure;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var config = builder.Configuration;
var env = builder.Environment;

// ✅ Read connection string from environment variable
var actualConnectionString = Environment.GetEnvironmentVariable("QueueDbConnectionString");
if (string.IsNullOrWhiteSpace(actualConnectionString))
    throw new InvalidOperationException("QueueDbConnectionString environment variable is missing.");

Console.WriteLine($"[ENV: {env.EnvironmentName}] QueueDbConnectionString Loaded: true");

// ✅ Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// ✅ Register services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(config, actualConnectionString);

// ✅ JWT Authentication
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

// ✅ CORS Setup
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ✅ Swagger with Bearer Token Support
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

    // ✅ Use the API Gateway base path for Swagger
    options.AddServer(new OpenApiServer
    {
        Url = "https://localhost:7260/queue"
    });

});

// ✅ Add controllers
builder.Services.AddControllers();

// ✅ Build the app
var app = builder.Build();

// ✅ Configure middleware
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger(c =>
    //{
    //    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    //    {
    //        var pathBase = "/queue";
    //        swaggerDoc.Servers = new List<OpenApiServer>
    //        {
    //            new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{pathBase}" }
    //        };
    //    });
    //});

    //app.UseSwaggerUI(c =>
    //{
    //    c.SwaggerEndpoint("/queue/swagger/v1/swagger.json", "Queue API V1");
    //    c.RoutePrefix = "swagger";
    //});

    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/queue/swagger/v1/swagger.json", "Queue API V1");
        c.RoutePrefix = "swagger";
    });

}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
