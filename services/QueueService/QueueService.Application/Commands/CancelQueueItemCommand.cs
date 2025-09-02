using MediatR;
using QueueService.Application.Common.Models;
using System;

namespace QueueService.Application.Commands
{
    public class CancelQueueItemCommand : IRequest<Result<Guid>>
    {
        public Guid QueueItemId { get; set; }

        public CancelQueueItemCommand(Guid queueItemId)
        {
            QueueItemId = queueItemId;
        }
    }
}
