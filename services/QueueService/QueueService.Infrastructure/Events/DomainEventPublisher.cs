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

            var eventType = @event.GetType().Name; // ✅ gets concrete class name

        {
        }

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

            // Optional: MediatR in-process publishing (after external message sent)
            try
            {
            }
            catch (Exception ex)
            {
            }
        }
    }
}
