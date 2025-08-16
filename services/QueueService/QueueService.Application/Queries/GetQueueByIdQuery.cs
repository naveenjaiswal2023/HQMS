using MediatR;
using QueueService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.Queries
{
    public class GetQueueByIdQuery : IRequest<QueueDetailsDto>
    {
        public Guid Id { get; set; }

        public GetQueueByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
