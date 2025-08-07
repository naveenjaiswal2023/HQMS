using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.Commands
{
    public class CancelQueueItemCommand : IRequest<Guid>
    {
        public Guid QueueItemId { get; set; }
    }
}
