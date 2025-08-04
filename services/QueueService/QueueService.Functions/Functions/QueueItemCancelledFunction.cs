//using Azure.Messaging.ServiceBus;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Extensions.Logging;
//using QueueService.Domain.Events;
//using System;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace QueueService.Functions.Functions;

//public class QueueItemCancelledHandler
//{
//    private readonly ILogger _logger;

//    public QueueItemCancelledHandler(ILoggerFactory loggerFactory)
//    {
//        _logger = loggerFactory.CreateLogger<QueueItemCancelledHandler>();
//    }

//    [Function("QueueItemCancelledHandler")]
//    public async Task RunAsync(
//        [ServiceBusTrigger("queueitem-events", "cancelled-subscription", Connection = "ServiceBusConnection")] string message)
//    {
//        var @event = JsonSerializer.Deserialize<QueueItemCancelledEvent>(message);
//        _logger.LogInformation("QueueItemCancelledHandler received event for QueueItemId: {QueueItemId}", @event?.QueueItemId);
//        // TODO: Update status in dashboard
//    }
//}