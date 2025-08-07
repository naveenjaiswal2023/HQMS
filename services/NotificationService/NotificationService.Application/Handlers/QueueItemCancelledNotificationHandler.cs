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
    public class QueueItemCancelledNotificationHandler : IEventHandler<QueueItemCancelledEvent>
    {
        private readonly INotificationSender _notificationSender;

        public QueueItemCancelledNotificationHandler(INotificationSender notificationSender)
        {
            _notificationSender = notificationSender;
        }

        public Task HandleAsync(QueueItemCancelledEvent @event, CancellationToken cancellationToken = default)
        {
            return _notificationSender.SendNotificationAsync(@event);
        }
    }

}
