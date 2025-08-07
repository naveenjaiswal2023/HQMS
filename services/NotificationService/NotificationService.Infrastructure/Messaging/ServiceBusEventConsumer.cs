using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Messaging
{
    public class ServiceBusEventConsumer<TEvent> : BackgroundService where TEvent : class, IDomainEvent
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ServiceBusEventConsumer<TEvent>> _logger;

        public ServiceBusEventConsumer(
            ServiceBusClient client,
            IServiceScopeFactory scopeFactory,
            ILogger<ServiceBusEventConsumer<TEvent>> logger,
            IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            var topicName = configuration[$"ServiceBus:Topics:NotificationEvents"];
            var subscriptionName = configuration[$"ServiceBus:Subscriptions:{typeof(TEvent).Name}"];

            if (string.IsNullOrWhiteSpace(topicName) || string.IsNullOrWhiteSpace(subscriptionName))
            {
                throw new InvalidOperationException($"Missing Service Bus topic/subscription config for {typeof(TEvent).Name}");
            }

            _processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += ProcessMessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            return _processor.StartProcessingAsync(stoppingToken);
        }

        private async Task ProcessMessageHandler(ProcessMessageEventArgs args)
        {
            try
            {
                var body = args.Message.Body.ToString();
                var @event = JsonSerializer.Deserialize<TEvent>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (@event != null)
                {
                    _logger.LogInformation("📩 Received {EventType} with QueueItemId: {Id}", typeof(TEvent).Name, GetQueueItemId(@event));

                    using var scope = _scopeFactory.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TEvent>>();
                    await handler.HandleAsync(@event);
                }

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing message of type {EventType}", typeof(TEvent).Name);
            }
        }

        private static Guid? GetQueueItemId(object @event)
        {
            var prop = @event.GetType().GetProperty("QueueItemId");
            return prop?.GetValue(@event) as Guid?;
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "❌ Service Bus processing error for {EventType}", typeof(TEvent).Name);
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }
    }

}
