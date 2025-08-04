using AuthService.Domain.Events;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Handlers.Events
{
    public class UserLoggedInEventHandler : INotificationHandler<UserLoggedInEvent>
    {
        private readonly ILogger<UserLoggedInEventHandler> _logger;
        private readonly IAzureServiceBusPublisher _busPublisher;

        public UserLoggedInEventHandler(
            ILogger<UserLoggedInEventHandler> logger,
            IAzureServiceBusPublisher busPublisher)
        {
            _logger = logger;
            _busPublisher = busPublisher;
        }

        public async Task Handle(UserLoggedInEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"User {notification.UserId} logged in at {notification.LoginTime:u}");

            try
            {
                // Publish to Azure Service Bus
                await _busPublisher.PublishAsync(notification, cancellationToken);
                _logger.LogInformation("Published UserLoggedInEvent to Azure Service Bus successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish UserLoggedInEvent to Azure Service Bus.");
            }
        }
    }
}
