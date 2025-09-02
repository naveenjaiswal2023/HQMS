using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Yarp.ReverseProxy.Model;
using Yarp.ReverseProxy.Transforms;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ---------- Configuration ----------
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// ---------- Serilog ----------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Host.UseSerilog();

// ---------- JWT Authentication ----------
var jwtSection = builder.Configuration.GetSection("JwtSettings");
var signingKey = jwtSection.GetValue<string>("Key")
                 ?? throw new InvalidOperationException("JwtSettings:Key required");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection.GetValue<string>("Issuer"),
            ValidateAudience = true,
            ValidAudience = jwtSection.GetValue<string>("Audience"),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Log.Warning("[JWT] Authentication failed: {Message}", ctx.Exception?.Message);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
});

// ---------- Rate Limiter ----------
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
    {
        var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 5,
            AutoReplenishment = true
        });
    });

    options.RejectionStatusCode = 429;
});

// ---------- YARP Reverse Proxy ----------
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transformBuilder =>
    {
        transformBuilder.AddRequestTransform(async transform =>
        {
            var httpContext = transform.HttpContext;

            // Forward Authorization header if present
            if (httpContext.Request.Headers.TryGetValue("Authorization", out var auth) &&
                !string.IsNullOrWhiteSpace(auth))
            {
                transform.ProxyRequest.Headers.Remove("Authorization");
                transform.ProxyRequest.Headers.TryAddWithoutValidation("Authorization", auth.ToString());
            }

            // Ensure X-Correlation-ID
            if (!httpContext.Request.Headers.ContainsKey("X-Correlation-ID"))
                httpContext.Request.Headers["X-Correlation-ID"] = Guid.NewGuid().ToString();

            if (httpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var corr))
                transform.ProxyRequest.Headers.TryAddWithoutValidation("X-Correlation-ID", corr.ToString());

            await Task.CompletedTask;
        });
    });

// ---------- Swagger ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HQMS API Gateway", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ---------- Build App ----------
var app = builder.Build();

// ---------- Middleware ----------
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// ---------- Swagger UI ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // ✅ Add ALL services here
        c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "Auth Service");
        c.SwaggerEndpoint("/appointments/swagger/v1/swagger.json", "Appointment Service");
        c.SwaggerEndpoint("/queue/swagger/v1/swagger.json", "Queue Service");
        c.SwaggerEndpoint("/patients/swagger/v1/swagger.json", "Patient Service");
        c.SwaggerEndpoint("/payments/swagger/v1/swagger.json", "Payment Service");
        c.RoutePrefix = "swagger";
    });
}

// ---------- Root redirect ----------
app.MapGet("/", ctx =>
{
    ctx.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

// ---------- Reverse Proxy ----------
app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use(async (context, next) =>
    {
        var proxyFeature = context.Features.Get<IReverseProxyFeature>();
        var routeConfig = proxyFeature?.Route;

        bool allowAnonymous = false;

        if (routeConfig?.Config?.Metadata?.TryGetValue("AllowAnonymous", out var value) == true &&
            string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
        {
            allowAnonymous = true;
        }

        if (allowAnonymous)
        {
            await next();
            return;
        }

        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
            return;
        }

        await next();
    });
});

app.Run();
