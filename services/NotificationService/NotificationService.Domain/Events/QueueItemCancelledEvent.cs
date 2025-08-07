using NotificationService.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Events
{
    public class QueueItemCancelledEvent : IDomainEvent
    {
        public Guid QueueItemId { get; set; }
        public Guid DoctorId { get; set; }
        public string CancelledBy { get; set; } // "Patient", "POD", etc.
        public DateTime CancelledAt { get; set; }

        public DateTime OccurredOn => throw new NotImplementedException();
    }
}
