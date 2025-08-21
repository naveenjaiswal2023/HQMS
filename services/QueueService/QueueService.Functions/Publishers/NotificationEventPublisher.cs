using Azure.Messaging.ServiceBus;
using Polly;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace QueueService.Functions.Publishers
{
    public class NotificationEventPublisher
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IAsyncPolicy _retryPolicy;
        private readonly IAsyncPolicy _circuitBreakerPolicy;
        private readonly ILogger<NotificationEventPublisher> _logger;
        private readonly string _topicName = "notification.events.topic";

        public NotificationEventPublisher(
            ServiceBusClient serviceBusClient,
            IAsyncPolicy retryPolicy,
            IAsyncPolicy circuitBreakerPolicy,
            ILogger<NotificationEventPublisher> logger)
        {
            _serviceBusClient = serviceBusClient ?? throw new ArgumentNullException(nameof(serviceBusClient));
            _retryPolicy = retryPolicy ?? throw new ArgumentNullException(nameof(retryPolicy));
            _circuitBreakerPolicy = circuitBreakerPolicy ?? throw new ArgumentNullException(nameof(circuitBreakerPolicy));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        {
            var combinedPolicy = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy);

            await combinedPolicy.ExecuteAsync(async ct =>
            {
                var jsonMessage = JsonSerializer.Serialize(@event);
                var message = new ServiceBusMessage(jsonMessage)
                {
                    ContentType = "application/json",
                    Subject = typeof(TEvent).Name
                };

                // Create sender per call (safe for Functions, avoids disposed sender issues)
                await using var sender = _serviceBusClient.CreateSender(_topicName);

                await sender.SendMessageAsync(message, ct);

                _logger.LogInformation("📤 Successfully published {EventType}", typeof(TEvent).Name);

            }, cancellationToken);
        }
    }
}
