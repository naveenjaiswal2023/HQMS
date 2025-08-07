using NotificationService.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Events
{
    public class QueueItemSkippedEvent : IDomainEvent
    {
        public Guid QueueItemId { get; set; }
        public Guid DoctorId { get; set; }
        public string Reason { get; set; }
        public DateTime SkippedAt { get; set; }

        public DateTime OccurredOn => throw new NotImplementedException();
    }
}
