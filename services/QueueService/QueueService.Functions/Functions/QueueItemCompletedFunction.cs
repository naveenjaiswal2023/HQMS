using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using QueueService.Domain.Events;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace QueueService.Functions.Functions
{
    public class QueueItemCompletedFunction
    {
        private readonly ILogger _logger;

        public QueueItemCompletedFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<QueueItemCompletedFunction>();
        }

        [Function("QueueItemCompletedFunction")]
        public async Task RunAsync(
            [ServiceBusTrigger("queue.patient.completed.topic", "queue.patient.completed.sub", Connection = "ServiceBusConnection")]
            string message,
            FunctionContext context)
        {
            try
            {
                var @event = JsonSerializer.Deserialize<QueueItemCompletedEvent>(message);

                if (@event == null)
                {
                    _logger.LogWarning("QueueItemCompletedEvent deserialized as null.");
                    return;
                }

                _logger.LogInformation("QueueItemCompletedHandler received event for QueueItemId: {QueueItemId}", @event.QueueItemId);

                // ✅ TODO: Notify UI via SignalR or update read model
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize QueueItemCompletedEvent message: {Message}", message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception while processing QueueItemCompletedEvent.");
            }
        }
    }
}
