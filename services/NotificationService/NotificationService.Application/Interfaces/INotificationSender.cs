using NotificationService.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Interfaces
{
    public interface INotificationSender
    {
        Task SendQueueCalledNotificationAsync(QueueItemCalledEvent evt);
    }
}
