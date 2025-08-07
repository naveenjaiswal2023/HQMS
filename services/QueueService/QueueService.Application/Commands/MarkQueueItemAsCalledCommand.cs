using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.Commands
{
    public class MarkQueueItemAsCalledCommand : IRequest<Guid>
    {
        public Guid QueueItemId { get; set; }
        public DateTime CalledAt { get; set; }
        public MarkQueueItemAsCalledCommand(Guid queueItemId, DateTime calledAt)
        {
            QueueItemId = queueItemId;
            CalledAt = calledAt;
        }
    }
}
