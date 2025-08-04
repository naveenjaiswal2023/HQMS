using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Events;
using System.Text.Json;

namespace NotificationService.Infrastructure.Messaging;

public class QueueItemCalledConsumer : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<QueueItemCalledConsumer> _logger;

    public QueueItemCalledConsumer(
        ServiceBusClient client,
        IServiceScopeFactory scopeFactory,
        ILogger<QueueItemCalledConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        _processor = client.CreateProcessor("notification.events.topic", "notification.events.sub", new ServiceBusProcessorOptions());
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += ProcessMessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;

        return _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task ProcessMessageHandler(ProcessMessageEventArgs args)
    {
        try
        {
            var body = args.Message.Body.ToString();
            var evt = JsonSerializer.Deserialize<QueueItemCalledEvent>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (evt != null)
            {
                _logger.LogInformation("📩 Received QueueItemCalledEvent for QueueItemId: {QueueItemId}", evt.QueueItemId);

                using var scope = _scopeFactory.CreateScope();
                var notificationSender = scope.ServiceProvider.GetRequiredService<INotificationSender>();

                await notificationSender.SendQueueCalledNotificationAsync(evt);
            }

            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error processing QueueItemCalledEvent.");
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "❌ ServiceBus processing error.");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        await _processor.DisposeAsync();
    }
}
