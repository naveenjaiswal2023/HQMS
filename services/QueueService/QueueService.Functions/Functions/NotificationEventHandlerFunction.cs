using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using QueueService.Domain.Events;
using QueueService.Functions.Publishers;
using System.Text.Json;

namespace QueueService.Functions.Functions;

public class NotificationEventHandlerFunction
{
    private readonly ILogger _logger;
    private readonly NotificationEventPublisher _notificationPublisher;

    public NotificationEventHandlerFunction(ILoggerFactory loggerFactory, ServiceBusClient serviceBusClient)
    {
        _logger = loggerFactory.CreateLogger<NotificationEventHandlerFunction>();
        _notificationPublisher = new NotificationEventPublisher(serviceBusClient);
    }

    [Function("HandleQueueItemCalledEvent")]
    public async Task HandleQueueItemCalledEvent(
        [ServiceBusTrigger("queue.patient.events.topic", "queue.patient.called.sub", Connection = "ServiceBusConnection")]
        string messageBody)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _logger.LogInformation("🔔 Received message: {MessageBody}", messageBody);
        var @event = JsonSerializer.Deserialize<QueueItemCalledEvent>(messageBody,options);
        if (@event == null)
        {
            _logger.LogWarning("❌ Deserialization failed for QueueItemCalledEvent.");
            return;
        }

        _logger.LogInformation("📥 Handling QueueItemCalledEvent for QueueItemId: {QueueItemId}", @event.QueueItemId);
        await _notificationPublisher.PublishAsync(@event);
    }

    [Function("HandleQueueItemCompletedEvent")]
    public async Task HandleQueueItemCompletedEvent(
        [ServiceBusTrigger("queue.patient.events.topic", "queue.patient.completed.sub", Connection = "ServiceBusConnection")]
        string messageBody)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _logger.LogInformation("🔔 Received message: {MessageBody}", messageBody,options);
        var @event = JsonSerializer.Deserialize<QueueItemCompletedEvent>(messageBody);
        if (@event == null)
        {
            _logger.LogWarning("❌ Deserialization failed for QueueItemCompletedEvent.");
            return;
        }

        _logger.LogInformation("📥 Handling QueueItemCompletedEvent for QueueItemId: {QueueItemId}", @event.QueueItemId);
        await _notificationPublisher.PublishAsync(@event);
    }

    [Function("HandleQueueItemCancelledEvent")]
    public async Task HandleQueueItemCancelledEvent(
        [ServiceBusTrigger("queue.patient.events.topic", "queue.patient.cancelled.sub", Connection = "ServiceBusConnection")]
        string messageBody)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _logger.LogInformation("🔔 Received message: {MessageBody}", messageBody);
        var @event = JsonSerializer.Deserialize<QueueItemCancelledEvent>(messageBody, options);
        if (@event == null)
        {
            _logger.LogWarning("❌ Deserialization failed for QueueItemCancelledEvent.");
            return;
        }

        _logger.LogInformation("📥 Handling QueueItemCancelledEvent for QueueItemId: {QueueItemId}", @event.QueueItemId);
        await _notificationPublisher.PublishAsync(@event);
    }

    [Function("HandleQueueItemSkippedEvent")]
    public async Task HandleQueueItemSkippedEvent(
        [ServiceBusTrigger("queue.patient.events.topic", "queue.patient.skipped.sub", Connection = "ServiceBusConnection")]
        string messageBody)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _logger.LogInformation("🔔 Received message: {MessageBody}", messageBody);
        var @event = JsonSerializer.Deserialize<QueueItemSkippedEvent>(messageBody,options);
        if (@event == null)
        {
            _logger.LogWarning("❌ Deserialization failed for QueueItemSkippedEvent.");
            return;
        }

        _logger.LogInformation("📥 Handling QueueItemSkippedEvent for QueueItemId: {QueueItemId}", @event.QueueItemId);
        await _notificationPublisher.PublishAsync(@event);
    }
}
