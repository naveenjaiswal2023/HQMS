using NotificationService.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Events
{
    public class QueueCompletedEvent : IDomainEvent
    {
        public Guid QueueId { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime CompletedAt { get; set; }

        public DateTime OccurredOn => throw new NotImplementedException();
    }
}
