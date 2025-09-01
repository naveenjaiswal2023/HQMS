using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientService.Application.Commands;
using PatientService.Application.Common.Behaviours;
using PatientService.Application.Interfaces;
using PatientService.Application.Services;
using PatientService.Application.Validators;
using PatientService.Domain.Interfaces;
using PatientService.Infrastructure;
using PatientService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<PatientDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PatientDbConnectionString")));

// MediatR with behaviors
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(RegisterPatientWithPaymentCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
});

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(RegisterPatientWithPaymentCommandValidator).Assembly);

// Payment Service Client
builder.Services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["PaymentService:BaseUrl"]);
});

// Health Checks
//builder.Services.AddHealthChecks()
//    .AddCheck<DatabaseHealthCheck>("database");

// API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Configuration.GetConnectionString("DefaultConnection"));

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