using HQMS.QueueService.Domain.Entities;
using MediatR;

namespace HQMS.QueueService.Application.Commands
{
    public class ProcessQueueItemCommand : IRequest<bool>
    {
        public Guid QueueItemId { get; set; }

        public ProcessQueueItemCommand(Guid queueItemId)
        {
            QueueItemId = queueItemId;
        }
    }
}
