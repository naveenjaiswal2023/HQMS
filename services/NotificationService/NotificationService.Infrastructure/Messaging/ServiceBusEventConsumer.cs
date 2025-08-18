using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Interface;
using Polly;
using Polly.CircuitBreaker;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Messaging
{
    public class ServiceBusEventConsumer<TEvent> : BackgroundService where TEvent : class, IDomainEvent
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ServiceBusEventConsumer<TEvent>> _logger;
        private readonly IAsyncPolicy _policy;
        private readonly int _maxDeliveryCountBeforeDLQ;
        private readonly bool _deadLetterOnDeserializationFailure;

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ServiceBusEventConsumer(
            ServiceBusClient client,
            IServiceScopeFactory scopeFactory,
            ILogger<ServiceBusEventConsumer<TEvent>> logger,
            IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            var topicName = configuration["ServiceBus:Topics:NotificationEvents"];
            var subscriptionName = configuration[$"ServiceBus:Subscriptions:{typeof(TEvent).Name}"];

            if (string.IsNullOrWhiteSpace(topicName) || string.IsNullOrWhiteSpace(subscriptionName))
                throw new InvalidOperationException($"Missing Service Bus topic/subscription config for {typeof(TEvent).Name}");

            _processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = configuration.GetValue<int?>("ServiceBus:Processing:MaxConcurrentCalls") ?? 8,
                PrefetchCount = configuration.GetValue<int?>("ServiceBus:Processing:PrefetchCount") ?? 32,
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5) // prevent lock expiration
            });

            _maxDeliveryCountBeforeDLQ = configuration.GetValue<int?>("ServiceBus:Processing:MaxAttemptsBeforeDeadLetter") ?? 5;
            _deadLetterOnDeserializationFailure = configuration.GetValue<bool?>("ServiceBus:Processing:DeadLetterOnDeserializationFailure") ?? true;

            // --- Resiliency policies ---
            var retry = Policy
                .Handle<Exception>(IsTransient)
                .WaitAndRetryAsync(3,
                    attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    (ex, delay, attempt, ctx) =>
                        _logger.LogWarning(ex,
                            "Retry {Attempt} after {Delay} handling {EventType}",
                            attempt, delay, typeof(TEvent).Name));

            var breaker = Policy
                .Handle<Exception>(IsTransient)
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.5,
                    samplingDuration: TimeSpan.FromSeconds(30),
                    minimumThroughput: 8,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (ex, breakDelay) =>
                        _logger.LogWarning(ex, "Circuit OPEN for {EventType} for {Delay}", typeof(TEvent).Name, breakDelay),
                    onReset: () =>
                        _logger.LogInformation("Circuit RESET for {EventType}", typeof(TEvent).Name),
                    onHalfOpen: () =>
                        _logger.LogInformation("Circuit HALF-OPEN for {EventType}", typeof(TEvent).Name));

            var timeout = Policy
                .TimeoutAsync(TimeSpan.FromSeconds(30),
                    Polly.Timeout.TimeoutStrategy.Pessimistic,
                    (ctx, ts, task) =>
                    {
                        _logger.LogError("Timeout after {Seconds}s in handler for {EventType}", ts.TotalSeconds, typeof(TEvent).Name);
                        return Task.CompletedTask;
                    });

            var bulkhead = Policy
                .BulkheadAsync(
                    maxParallelization: 20,
                    maxQueuingActions: int.MaxValue,
                    onBulkheadRejectedAsync: ctx =>
                    {
                        _logger.LogWarning("Bulkhead limit reached for {EventType}", typeof(TEvent).Name);
                        return Task.CompletedTask;
                    });

            // Retry inside circuit breaker → wrapped with timeout & bulkhead
            _policy = Policy.WrapAsync(bulkhead, timeout, Policy.WrapAsync(retry, breaker));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += ProcessMessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;
            return _processor.StartProcessingAsync(stoppingToken);
        }

        private async Task ProcessMessageHandler(ProcessMessageEventArgs args)
        {
            var messageId = args.Message.MessageId;
            var deliveryCount = args.Message.DeliveryCount;

            try
            {
                // Deserialize
                TEvent? @event;
                try
                {
                    @event = args.Message.Body.ToObjectFromJson<TEvent>(SerializerOptions);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "❌ Deserialization failed for {EventType}, MessageId={MessageId}",
                        typeof(TEvent).Name, messageId);

                    if (_deadLetterOnDeserializationFailure)
                        await args.DeadLetterMessageAsync(args.Message, "DeserializationFailed", ex.Message);
                    else
                        await args.AbandonMessageAsync(args.Message);

                    return;
                }

                //if (@event is null)
                //{
                //    _logger.LogWarning("Null event for {EventType}, MessageId={MessageId}", typeof(TEvent).Name, messageId);
                //    await args.DeadLetterMessageAsync(args.Message, "NullEvent", "Deserialized event was null");
                //    return;
                //}
                if (string.IsNullOrWhiteSpace((@event as dynamic)?.DoctorId))
                {
                    _logger.LogWarning("Invalid event: DoctorId is null or empty. MessageId={MessageId}", messageId);
                    await args.DeadLetterMessageAsync(args.Message, "ValidationFailed", "DoctorId cannot be null or empty");
                    return;
                }

                if (deliveryCount > _maxDeliveryCountBeforeDLQ)
                {
                    _logger.LogWarning("Dead-lettering {EventType}, MessageId={MessageId} after {DeliveryCount} deliveries",
                        typeof(TEvent).Name, messageId, deliveryCount);
                    await args.DeadLetterMessageAsync(args.Message, "MaxDeliveryExceeded", $"Exceeded {_maxDeliveryCountBeforeDLQ} attempts");
                    return;
                }

                // Run handler with resiliency
                await _policy.ExecuteAsync(async ct =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TEvent>>();
                    await handler.HandleAsync(@event!, ct);
                }, args.CancellationToken);

                await args.CompleteMessageAsync(args.Message);
            }
            catch (ServiceBusException sbEx) when (sbEx.Reason == ServiceBusFailureReason.MessageLockLost)
            {
                // Don’t try to complete/abandon after lock expired
                _logger.LogWarning(sbEx, "⚠️ Message lock lost for {EventType}, MessageId={MessageId}", typeof(TEvent).Name, messageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Error handling {EventType}, MessageId={MessageId}, DeliveryCount={DeliveryCount}",
                    typeof(TEvent).Name, messageId, deliveryCount);

                await args.AbandonMessageAsync(args.Message);
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception,
                "❌ Service Bus error for {EventType}, Entity={Entity}, Source={Source}",
                typeof(TEvent).Name, args.EntityPath, args.ErrorSource);
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _processor.ProcessMessageAsync -= ProcessMessageHandler;
            _processor.ProcessErrorAsync -= ErrorHandler;
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }

        private static bool IsTransient(Exception ex) =>
            ex is TimeoutException ||
            ex is TaskCanceledException ||
            (ex is ServiceBusException sbEx && sbEx.IsTransient);
    }
}
