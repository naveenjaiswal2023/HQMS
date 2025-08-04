using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.Commands
{
    public class MarkQueueItemAsCompletedCommand : IRequest<Guid>
    {
        public Guid QueueItemId { get; set; }
        public DateTime CompletedAt { get; set; }
        public MarkQueueItemAsCompletedCommand(Guid queueItemId, DateTime completedAt)
        {
            QueueItemId = queueItemId;
            CompletedAt = completedAt;
        }
    }
}
