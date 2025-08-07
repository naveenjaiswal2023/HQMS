using QueueService.Domain.Common;
using QueueService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Domain.Events
{
    public class QueueItemPositionUpdatedEvent : BaseDomainEvent, IDomainEvent
    {
        public Guid QueueItemId { get; }
        public int NewPosition { get; }
        public DateTime UpdatedAt { get; } = DateTime.UtcNow;
        public QueueItemPositionUpdatedEvent(Guid queueItemId, int newPosition)
        {
            QueueItemId = queueItemId;
            NewPosition = newPosition;
        }
    }
}
