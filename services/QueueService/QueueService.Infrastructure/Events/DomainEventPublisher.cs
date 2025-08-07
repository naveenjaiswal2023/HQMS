using Azure.Messaging.ServiceBus;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QueueService.Domain.Interfaces;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService.Infrastructure.Events
{
    public class DomainEventPublisher : IDomainEventPublisher
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DomainEventPublisher> _logger;
        private readonly ServiceBusClient _client;
        private readonly IConfiguration _config;

        public DomainEventPublisher(
            IMediator mediator,
            ILogger<DomainEventPublisher> logger,
            ServiceBusClient client,
            IConfiguration config)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IDomainEvent
        {
            if (@event == null)
            {
                _logger.LogWarning("Attempted to publish a null domain event.");
                throw new ArgumentNullException(nameof(@event));
            }

            var eventType = @event.GetType().Name;
            var topicName = _config["ServiceBus:Topics:PatientQueueEvents"];

            if (string.IsNullOrWhiteSpace(topicName))
            {
                _logger.LogError("No unified topic configured under 'PatientQueueEvents'");
                throw new InvalidOperationException("No topic configured for PatientQueueEvents");
            }

            string jsonPayload;
            try
            {
                jsonPayload = JsonSerializer.Serialize(@event, @event.GetType(), new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    Converters = { new JsonStringEnumConverter() }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to serialize event of type {EventType}", eventType);
                throw;
            }

            var message = new ServiceBusMessage(jsonPayload)
            {
                ContentType = "application/json",
                Subject = eventType
            };
            message.ApplicationProperties["Type"] = eventType;

            try
            {
                await using var sender = _client.CreateSender(topicName);
                await sender.SendMessageAsync(message, cancellationToken);
                _logger.LogInformation("✅ Published {EventType} to topic {TopicName}", eventType, topicName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to publish {EventType} to topic {TopicName}", eventType, topicName);
                throw;
            }

            try
            {
                await _mediator.Publish(@event, cancellationToken);
                _logger.LogDebug("📢 In-process MediatR event dispatched for {EventType}", eventType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MediatR dispatch failed for {EventType}", eventType);
            }
        }
    }
}
