using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QueueService.Domain.Events;
using QueueService.Functions.Publishers;
using System.Text.Json;
using System.Threading;

namespace QueueService.Functions.Functions
{
    public class NotificationEventHandlerFunction
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public NotificationEventHandlerFunction(ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory)
        {
            _logger = loggerFactory.CreateLogger<NotificationEventHandlerFunction>();
            _scopeFactory = scopeFactory;
        }

        [Function("HandleQueueItemCalledEvent")]
        public async Task HandleQueueItemCalledEvent(
            [ServiceBusTrigger("queue.patient.events.topic", "queue.patient.called.sub", Connection = "ServiceBusConnection")]
            string messageBody, CancellationToken cancellationToken)
        {
            await HandleEventAsync<QueueItemCalledEvent>(messageBody, "QueueItemCalledEvent", cancellationToken);
        }

        [Function("HandleQueueItemCompletedEvent")]
        public async Task HandleQueueItemCompletedEvent(
            [ServiceBusTrigger("queue.patient.events.topic", "queue.patient.completed.sub", Connection = "ServiceBusConnection")]
            string messageBody, CancellationToken cancellationToken)
        {
            await HandleEventAsync<QueueItemCompletedEvent>(messageBody, "QueueItemCompletedEvent", cancellationToken);
        }

        [Function("HandleQueueItemCancelledEvent")]
        public async Task HandleQueueItemCancelledEvent(
            [ServiceBusTrigger("queue.patient.events.topic", "queue.patient.cancelled.sub", Connection = "ServiceBusConnection")]
            string messageBody, CancellationToken cancellationToken)
        {
            await HandleEventAsync<QueueItemCancelledEvent>(messageBody, "QueueItemCancelledEvent", cancellationToken);
        }

        [Function("HandleQueueItemSkippedEvent")]
        public async Task HandleQueueItemSkippedEvent(
            [ServiceBusTrigger("queue.patient.events.topic", "queue.patient.skipped.sub", Connection = "ServiceBusConnection")]
            string messageBody, CancellationToken cancellationToken)
        {
            await HandleEventAsync<QueueItemSkippedEvent>(messageBody, "QueueItemSkippedEvent", cancellationToken);
        }

        private async Task HandleEventAsync<T>(string messageBody, string eventType, CancellationToken cancellationToken)
            where T : class
        {
            using var activity = new System.Diagnostics.Activity($"Handle{eventType}").Start();

            try
            {
                _logger.LogInformation("🔔 Received {EventType} message: {MessageBody}", eventType, messageBody);

                var @event = JsonSerializer.Deserialize<T>(messageBody, JsonOptions);

                if (@event == null)
                {
                    _logger.LogWarning("❌ Deserialization failed for {EventType}", eventType);
                    return;
                }

                // resolve publisher per invocation
                using var scope = _scopeFactory.CreateScope();
                var publisher = scope.ServiceProvider.GetRequiredService<NotificationEventPublisher>();

                await publisher.PublishAsync(@event, cancellationToken);

                _logger.LogInformation("✅ Successfully processed {EventType}", eventType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing {EventType}. Body: {MessageBody}", eventType, messageBody);
                throw; // trigger retry
            }
        }
    }
}
