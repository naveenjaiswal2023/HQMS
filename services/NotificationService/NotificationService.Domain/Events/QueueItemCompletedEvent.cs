using NotificationService.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Events
{
    public class QueueItemCompletedEvent : IDomainEvent
    {
        public Guid QueueItemId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime CompletedAt { get; set; }

        public DateTime OccurredOn => throw new NotImplementedException();
    }
}
