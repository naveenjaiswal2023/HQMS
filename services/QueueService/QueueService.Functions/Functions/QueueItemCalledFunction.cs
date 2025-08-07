//using Azure.Messaging.ServiceBus;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Extensions.Logging;
//using QueueService.Domain.Events;
//using QueueService.Functions.Publishers;
//using System;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace QueueService.Functions.Functions;

//public class QueueItemCalledFunction
//{
//    private readonly ILogger _logger;
//    private readonly NotificationEventPublisher _notificationPublisher;

//    public QueueItemCalledFunction(ILoggerFactory loggerFactory, ServiceBusClient serviceBusClient)
//    {
//        _logger = loggerFactory.CreateLogger<QueueItemCalledFunction>();
//        _notificationPublisher = new NotificationEventPublisher(serviceBusClient);
//    }

//    [Function("QueueItemCalledFunction")]
//    public async Task RunAsync(
//        [ServiceBusTrigger("queue.patient.called.topic", "queue.patient.called.sub", Connection = "ServiceBusConnection")]
//        string message)
//    {
//        _logger.LogInformation("🔔 Received message: {Message}", message);

//        try
//        {
//            var options = new JsonSerializerOptions
//            {
//                PropertyNameCaseInsensitive = true
//            };

//            var @event = JsonSerializer.Deserialize<QueueItemCalledEvent>(message, options);

//            // Replace the problematic validation line with correct checks for Guid and DoctorName
//            if (@event == null || @event.QueueItemId == Guid.Empty || string.IsNullOrEmpty(@event.DoctorName))
//            {
//                _logger.LogWarning("⚠️ Invalid event data. Skipping processing.");
//                return;
//            }
            
//            _logger.LogInformation("✅ Validated event for QueueItemId: {QueueItemId}", @event.QueueItemId);

//            // Forward to NotificationService
//            await _notificationPublisher.PublishAsync(@event);
//            _logger.LogInformation("📨 Event forwarded to notification.events.topic");
//        }
//        catch (JsonException ex)
//        {
//            _logger.LogError(ex, "❌ Deserialization error.");
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "❌ Unhandled error in QueueItemCalledFunction.");
//        }
//    }
//}
