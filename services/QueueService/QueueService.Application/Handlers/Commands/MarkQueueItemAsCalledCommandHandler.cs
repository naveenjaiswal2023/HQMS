using MediatR;
using QueueService.Application.Commands;
using QueueService.Application.Exceptions;
using QueueService.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService.Application.Handlers.Commands
{
    public class MarkQueueItemAsCalledCommandHandler : IRequestHandler<MarkQueueItemAsCalledCommand, Guid>
    {
        private readonly IQueueItemRepository _queueItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkQueueItemAsCalledCommandHandler(IQueueItemRepository queueItemRepository, IUnitOfWork unitOfWork)
        {
            _queueItemRepository = queueItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(MarkQueueItemAsCalledCommand request, CancellationToken cancellationToken)
        {
            var item = await _queueItemRepository.GetByIdAsync(request.QueueItemId, cancellationToken);

            if (item == null)
                throw new NotFoundException("QueueItem", request.QueueItemId);

            item.MarkAsCalled();

            await _unitOfWork.SaveAsync(cancellationToken);

            return item.Id;
        }
    }
}
