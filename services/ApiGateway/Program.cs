using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Yarp.ReverseProxy.Transforms;
using System.Threading.RateLimiting;
using Yarp.ReverseProxy.Model;

var builder = WebApplication.CreateBuilder(args);

// ---------- Configuration ----------
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// ---------- Serilog ----------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// ---------- YARP (Load routes/clusters from config) ----------
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transformContext =>
    {
        // global request transform to ensure Authorization and Correlation ID forwarded
        transformContext.AddRequestTransform(async transform =>
        {
            var httpContext = transform.HttpContext;

            // Forward Authorization header explicitly if present
            if (httpContext.Request.Headers.TryGetValue("Authorization", out var auth) &&
                !string.IsNullOrEmpty(auth))
            {
                transform.ProxyRequest.Headers.TryAddWithoutValidation("Authorization", (string)auth);
            }

            // Ensure X-Correlation-ID exists and is forwarded
            if (!httpContext.Request.Headers.ContainsKey("X-Correlation-ID"))
            {
                httpContext.Request.Headers["X-Correlation-ID"] = Guid.NewGuid().ToString();
            }

            if (httpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var corr))
            {
                transform.ProxyRequest.Headers.TryAddWithoutValidation("X-Correlation-ID", (string)corr);
            }

            await Task.CompletedTask;
        });
    });

// ---------- JWT Authentication ----------
var jwtSection = builder.Configuration.GetSection("JwtSettings");
var signingKey = jwtSection.GetValue<string>("Key") ?? throw new InvalidOperationException("Jwt:Key required in configuration.");

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
    // default "Authenticated" policy—used below for proxy mapping
    options.AddPolicy("Authenticated", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});

// ---------- Rate Limiter (Partitioned by IP) ----------
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        // Example: 60 requests per minute per IP with small queue
        var lease = RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 5,
            AutoReplenishment = true
        });

        return lease;
    });

    options.RejectionStatusCode = 429;
    options.OnRejected = async (context, ct) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "application/json";
        var resp = new
        {
            error = "Too many requests",
            message = "Rate limit exceeded. Try again later.",
            retryAfterSeconds = 60
        };
        await context.HttpContext.Response.WriteAsJsonAsync(resp, ct);
    };
});

// ---------- Swagger (centralized gateway UI) ----------
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

var app = builder.Build();

// ---------- Global Exception Handling ----------
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Unhandled exception in API Gateway");
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "GatewayError", message = "Internal Gateway Error" });
    }
});

// ---------- Middleware pipeline ----------
app.UseSerilogRequestLogging(); // provides structured request logging
app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// ---------- Swagger endpoints (protected in production) ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "Auth Service");
        c.SwaggerEndpoint("/appointments/swagger/v1/swagger.json", "Appointment Service");
        c.SwaggerEndpoint("/queue/swagger/v1/swagger.json", "Queue Service");
        c.RoutePrefix = "swagger";
    });
}
else
{
    // In production, require authentication to access swagger UI
    app.MapGet("/swagger", async ctx =>
    {
        if (!ctx.User.Identity?.IsAuthenticated ?? true)
        {
            ctx.Response.StatusCode = 401;
            return;
        }
        ctx.Response.Redirect("/swagger/index.html");
    });

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "Auth Service");
        c.SwaggerEndpoint("/appointments/swagger/v1/swagger.json", "Appointment Service");
        c.SwaggerEndpoint("/queue/swagger/v1/swagger.json", "Queue Service");
        c.RoutePrefix = "swagger";
    });
}

// ---------- Root redirect to swagger ----------
app.MapGet("/", (HttpContext ctx) =>
{
    ctx.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

// ---------- Map Reverse Proxy and require authentication for proxied routes ----------
//app.MapReverseProxy()
//   .RequireAuthorization("Authenticated");
app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use(async (context, next) =>
    {
        var proxyFeature = context.Features.Get<IReverseProxyFeature>();
        var route = proxyFeature?.Route.Config;

        // If route metadata says AllowAnonymous = true, skip auth
        if (route?.Metadata?.TryGetValue("AllowAnonymous", out var allowAnon) == true &&
            allowAnon.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            await next();
            return;
        }

        // Otherwise enforce authentication
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
            return;
        }

        await next();
    });
});

// ---------- Start ----------
app.Run();
