using Azure.Messaging.ServiceBus;
using NotificationService.API.Middleware;
using NotificationService.Application.Handlers;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Events;
using NotificationService.Domain.Interface;
using NotificationService.Infrastructure.Messaging;
using NotificationService.Infrastructure.Services;
using NotificationService.SignalR.Hubs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// Add Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddSignalR();

builder.Services.AddSingleton(new ServiceBusClient(builder.Configuration["ServiceBus:ConnectionString"]));
builder.Services.AddScoped<INotificationSender, SignalRNotificationSender>();
//builder.Services.AddHostedService<QueueItemCalledConsumer>();
builder.Services.AddSignalR();
builder.Services.AddScoped<INotificationSender, SignalRNotificationSender>();


builder.Services.AddHostedService<ServiceBusEventConsumer<QueueItemCalledEvent>>();
builder.Services.AddHostedService<ServiceBusEventConsumer<QueueItemCompletedEvent>>();
builder.Services.AddHostedService<ServiceBusEventConsumer<QueueItemCancelledEvent>>();
builder.Services.AddHostedService<ServiceBusEventConsumer<QueueItemSkippedEvent>>();

builder.Services.AddScoped<IEventHandler<QueueItemCalledEvent>, QueueItemCalledNotificationHandler>();
builder.Services.AddScoped<IEventHandler<QueueItemCompletedEvent>, QueueItemCompletedNotificationHandler>();
builder.Services.AddScoped<IEventHandler<QueueItemCancelledEvent>, QueueItemCancelledNotificationHandler>();
builder.Services.AddScoped<IEventHandler<QueueItemSkippedEvent>, QueueItemSkippedNotificationHandler>();



builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true);
    });
});

// Middleware for exception handling
//builder.Services.AddTransient<ExceptionHandlingMiddleware>();
var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors();

app.MapHub<NotificationHub>("/notificationHub");

app.Run();
