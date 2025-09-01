using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientService.Application.Interfaces;
using PatientService.Application.Services;
using PaymentService.Application.Commands;
using PaymentService.Application.Common.Behaviours;
using PaymentService.Application.Interfaces;
using PaymentService.Application.Validators;
using PaymentService.Domain.Models.Payments;
using PaymentService.Infrastructure;
using PaymentService.Infrastructure.Models;
using PaymentService.Infrastructure.Persistence;
using PaymentService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(InitiateRegistrationPaymentCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
});

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(InitiateRegistrationPaymentCommandValidator).Assembly);

// Health Checks
//builder.Services.AddHealthChecks()
//    .AddCheck<DatabaseHealthCheck>("database");

// API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Configuration.GetConnectionString("PaymentDbConnectionString"));

var app = builder.Build();

// Middleware pipeline
//app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();
//app.MapHealthChecks("/health");

app.Run();