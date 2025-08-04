using Azure.Messaging.ServiceBus;
using NotificationService.Application.Interfaces;
using NotificationService.Infrastructure.Messaging;
using NotificationService.Infrastructure.Services;
using NotificationService.SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddSingleton(new ServiceBusClient(builder.Configuration["ServiceBus:ConnectionString"]));
builder.Services.AddScoped<INotificationSender, SignalRNotificationSender>();
builder.Services.AddHostedService<QueueItemCalledConsumer>();

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

var app = builder.Build();
app.UseCors();

app.MapHub<NotificationHub>("/notificationHub");

app.Run();
