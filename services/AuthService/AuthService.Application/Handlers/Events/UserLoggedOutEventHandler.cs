using AuthService.Domain.Events;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthService.Application.Handlers.Events
{
    public class UserLoggedOutEventHandler : INotificationHandler<UserLoggedOutEvent>
    {
        private readonly ILogger<UserLoggedOutEventHandler> _logger;
        private readonly IAzureServiceBusPublisher _publisher;

        public UserLoggedOutEventHandler(IAzureServiceBusPublisher publisher, ILogger<UserLoggedOutEventHandler> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Handle(UserLoggedOutEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var message = JsonSerializer.Serialize(notification);
                await _publisher.PublishAsync(notification, cancellationToken);
                _logger.LogInformation("Published UserLoggedOutEvent to Azure Service Bus successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish UserLoggedOutEvent to Azure Service Bus.");
            }
}
    }
}
