
using Azure.Messaging.ServiceBus;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymentService.Domain.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaymentService.Infrastructure.Events
{
    public class DomainEventPublisher : IDomainEventPublisher
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DomainEventPublisher> _logger;
        private readonly ServiceBusSender _sender; // Reused sender
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        public DomainEventPublisher(
            IMediator mediator,
            ILogger<DomainEventPublisher> logger,
            ServiceBusClient client,
            IConfiguration config)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var topicName = config["ServiceBus:Topics:PatientQueueEvents"]
                ?? throw new InvalidOperationException("No topic configured for PatientQueueEvents");

            // Create sender ONCE, reuse across PublishAsync calls
            _sender = client.CreateSender(topicName);
        }

        public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IDomainEvent
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            var eventType = @event.GetType().Name;
            string jsonPayload;

            try
            {
                jsonPayload = JsonSerializer.Serialize(@event, @event.GetType(), _serializerOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Serialization failed for {EventType}", eventType);
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
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Publishing cancelled for event: {EventType}", typeof(T).Name);
                    return;
                }
                _logger.LogDebug("📢 Publishing {EventType} to topic {TopicName}", eventType, _sender.EntityPath);

                await _sender.SendMessageAsync(message, cancellationToken);
                _logger.LogInformation("✅ Published {EventType} to topic {TopicName}", eventType, _sender.EntityPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to publish {EventType}", eventType);
                throw;
            }

            // Local MediatR publish
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Dispatching cancelled for event: {EventType}", typeof(T).Name);
                    return;
                }
                _logger.LogDebug("📢 Dispatching MediatR event: {EventType}", eventType);

                await _mediator.Publish(@event, cancellationToken);
                _logger.LogDebug("📢 MediatR dispatched {EventType}", eventType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MediatR dispatch failed for {EventType}", eventType);
            }
        }
    }
}
