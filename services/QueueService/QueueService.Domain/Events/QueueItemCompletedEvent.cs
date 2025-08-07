using QueueService.Domain.Common;
using QueueService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Domain.Events
{
    public class QueueItemCompletedEvent : BaseDomainEvent, IDomainEvent
    {
        public Guid QueueItemId { get; init; }
        public DateTime CompletedAt { get; init; } = DateTime.UtcNow;
        public QueueItemCompletedEvent(Guid queueItemId)
        {
            QueueItemId = queueItemId;
        }
    }
}
