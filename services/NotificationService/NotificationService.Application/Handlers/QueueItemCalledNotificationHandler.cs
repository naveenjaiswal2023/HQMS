using NotificationService.Application.Interfaces;
using NotificationService.Domain.Events;
using NotificationService.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Handlers
{
    public class QueueItemCalledNotificationHandler : IEventHandler<QueueItemCalledEvent>
    {
        private readonly INotificationSender _notificationSender;

        public QueueItemCalledNotificationHandler(INotificationSender notificationSender)
        {
            _notificationSender = notificationSender;
        }

        public Task HandleAsync(QueueItemCalledEvent @event, CancellationToken cancellationToken = default)
        {
            return _notificationSender.SendNotificationAsync(@event);
        }
    }
}
