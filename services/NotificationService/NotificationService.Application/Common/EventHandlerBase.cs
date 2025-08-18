using Polly;
using Polly.CircuitBreaker; // 👈 Required for CircuitBreakerAsync
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService.Application.Common
{
    public abstract class EventHandlerBase<TEvent>
    {
        private readonly IAsyncPolicy _retryPolicy;
        private readonly IAsyncPolicy _circuitBreakerPolicy;

        protected EventHandlerBase()
        {
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))
                );

            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                );
        }

        public async Task HandleWithPoliciesAsync(
            TEvent @event,
            Func<TEvent, CancellationToken, Task> handler,
            CancellationToken cancellationToken = default)
        {
            await _retryPolicy.WrapAsync(_circuitBreakerPolicy)
                              .ExecuteAsync(() => handler(@event, cancellationToken));
        }
    }
}
