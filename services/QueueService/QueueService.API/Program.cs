using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using QueueService.Application;
using QueueService.Infrastructure;


//using SharedInfrastructure.Settings;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var configuration = builder.Configuration;
var environment = builder.Environment;

// ✅ Build connection string
var connTemplate = configuration.GetConnectionString("QueueDbConnectionString")
    ?? Environment.GetEnvironmentVariable("QueueDbConnectionString");

var dbPassword = configuration["QmsDbPassword"]
    ?? Environment.GetEnvironmentVariable("QmsDbPassword");

if (string.IsNullOrEmpty(connTemplate))
    throw new InvalidOperationException("QueueDbConnectionString is missing.");

if (string.IsNullOrEmpty(dbPassword))
    throw new InvalidOperationException("QmsDbPassword is missing.");

var actualConnectionString = connTemplate.Replace("_QmsDbPassword_", dbPassword);
Console.WriteLine($"[ENV: {environment.EnvironmentName}] Connection String SET: {(!string.IsNullOrWhiteSpace(actualConnectionString)).ToString()}");

// ✅ Add services
builder.Services.AddApplicationServices();                     
builder.Services.AddInfrastructureServices(configuration);    

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

// ✅ Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ✅ Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Add controllers
builder.Services.AddControllers();

var app = builder.Build();

// ✅ Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Service API V1");
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
