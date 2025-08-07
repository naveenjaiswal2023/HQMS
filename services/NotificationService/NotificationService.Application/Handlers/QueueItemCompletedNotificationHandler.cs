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
    public class QueueItemCompletedNotificationHandler : IEventHandler<QueueItemCompletedEvent>
    {
        private readonly INotificationSender _notificationSender;

        public QueueItemCompletedNotificationHandler(INotificationSender notificationSender)
        {
            _notificationSender = notificationSender;
        }

        public Task HandleAsync(QueueItemCompletedEvent @event, CancellationToken cancellationToken = default)
        {
            return _notificationSender.SendNotificationAsync(@event);
        }
    }

}
