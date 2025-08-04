using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.Commands
{
    public class MarkQueueItemAsSkippedCommand : IRequest<Guid>
    {
        public Guid QueueItemId { get; set; }
        public DateTime SkippedAt { get; set; }
        public MarkQueueItemAsSkippedCommand(Guid queueItemId, DateTime skippedAt)
        {
            QueueItemId = queueItemId;
            SkippedAt = skippedAt;
        }
    }
}
