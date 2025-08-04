using AuthService.Domain.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AuthService.Infrastructure.Messaging
{
    public class AzureServiceBusPublisher : IAzureServiceBusPublisher
    {
        private readonly ServiceBusClient _client;
        private readonly IConfiguration _config;
        private readonly ILogger<AzureServiceBusPublisher> _logger;

        public AzureServiceBusPublisher(ServiceBusClient client, IConfiguration config, ILogger<AzureServiceBusPublisher> logger)
        {
            _client = client;
            _config = config;
            _logger = logger;
        }

        public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
        {
            var eventType = typeof(T).Name;

            // 🔁 Lookup topic from configuration
            var topicName = _config[$"ServiceBus:Topics:{eventType}"];

            if (string.IsNullOrEmpty(topicName))
            {
                _logger.LogError("No topic configured for event type: {EventType}", eventType);
                throw new InvalidOperationException($"No topic configured for event type {eventType}");
            }

            var json = JsonSerializer.Serialize(@event);
            var message = new ServiceBusMessage(json)
            {
                ContentType = "application/json",
                Subject = eventType
            };
            message.ApplicationProperties["Type"] = eventType;

            try
            {
                var sender = _client.CreateSender(topicName);
                await sender.SendMessageAsync(message, cancellationToken);
                _logger.LogInformation("Published event {EventType} to topic {TopicName}", eventType, topicName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish event {EventType} to topic {TopicName}", eventType, topicName);
                throw;
            }
        }
    }
}