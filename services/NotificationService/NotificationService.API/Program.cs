using Azure.Messaging.ServiceBus;
using NotificationService.API.Middleware;
using NotificationService.Application.Interfaces;
using NotificationService.Infrastructure.Messaging;
using NotificationService.Infrastructure.Services;
using NotificationService.SignalR.Hubs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// Add Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddSignalR();


{




app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
