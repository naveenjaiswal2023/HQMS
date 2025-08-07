//using System;
//using System.Threading.Tasks;
//using Azure.Messaging.ServiceBus;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Extensions.Logging;

//namespace QueueService.Functions.Functions;

//public class QueueItemSkippedHandler
//{
//    private readonly ILogger<QueueItemSkippedHandler> _logger;

//    public QueueItemSkippedHandler(ILogger<QueueItemSkippedHandler> logger)
//    {
//        _logger = logger;
//    }

//    [Function(nameof(QueueItemSkippedHandler))]
//    public async Task Run(
//        [ServiceBusTrigger("mytopic", "mysubscription", Connection = "")]
//        ServiceBusReceivedMessage message,
//        ServiceBusMessageActions messageActions)
//    {
//        _logger.LogInformation("Message ID: {id}", message.MessageId);
//        _logger.LogInformation("Message Body: {body}", message.Body);
//        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

//            // Complete the message
//        await messageActions.CompleteMessageAsync(message);
//    }
//}