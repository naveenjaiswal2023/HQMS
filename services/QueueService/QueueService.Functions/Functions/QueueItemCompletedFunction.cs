//using Azure.Messaging.ServiceBus;
//using MediatR;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Extensions.Logging;
//using QueueService.Domain.Events;
//using QueueService.Functions.Publishers;
//using System;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace QueueService.Functions.Functions
//{
//    public class QueueItemCompletedFunction
//    {
//        private readonly ILogger _logger;
//        //private readonly INotificationPublisher _notificationPublisher;
//        private readonly NotificationEventPublisher _notificationPublisher;

//        public QueueItemCompletedFunction(ILoggerFactory loggerFactory, ServiceBusClient serviceBusClient)
//        {
//            _logger = loggerFactory.CreateLogger<QueueItemCompletedFunction>();
//            _notificationPublisher = new NotificationEventPublisher(serviceBusClient);

//        }

//        [Function("QueueItemCompletedFunction")]
//        public async Task RunAsync(
//            [ServiceBusTrigger("queue.patient.completed.topic", "queue.patient.completed.sub", Connection = "ServiceBusConnection")]
//            string message,
//            FunctionContext context)
//        {
//            try
//            {
//                var @event = JsonSerializer.Deserialize<QueueItemCompletedEvent>(message);

//                if (@event == null)
//                {
//                    _logger.LogWarning("QueueItemCompletedEvent deserialized as null.");
//                    return;
//                }

//                _logger.LogInformation("QueueItemCompletedHandler received event for QueueItemId: {QueueItemId}", @event.QueueItemId);

//                // ✅ TODO: Notify UI via SignalR or update read model
//                // Forward to NotificationService
//                await _notificationPublisher.PublishAsync(@event);
//                _logger.LogInformation("📨 Event forwarded to notification.events.topic");
//            }
//            catch (JsonException ex)
//            {
//                _logger.LogError(ex, "Failed to deserialize QueueItemCompletedEvent message: {Message}", message);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Unhandled exception while processing QueueItemCompletedEvent.");
//            }
//        }
//    }
//}
