using MediatR;
using QueueService.Application.Commands;
using QueueService.Application.Exceptions; // Assuming this is where NotFoundException is
using QueueService.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService.Application.Handlers.Commands
{
    public class MarkQueueItemAsCompletedCommandHandler : IRequestHandler<MarkQueueItemAsCompletedCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueueItemRepository _queueRepository;

        public MarkQueueItemAsCompletedCommandHandler(IUnitOfWork unitOfWork, IQueueItemRepository queueRepository)
        {
            _unitOfWork = unitOfWork;
            _queueRepository = queueRepository;
        }

        public async Task<Guid> Handle(MarkQueueItemAsCompletedCommand request, CancellationToken cancellationToken)
        {
            var queueItem = await _queueRepository.GetByIdAsync(request.QueueItemId, cancellationToken);
            if (queueItem == null)
                throw new NotFoundException("QueueItem", request.QueueItemId);

            queueItem.MarkAsCompleted();

            await _unitOfWork.SaveAsync(cancellationToken);

            return queueItem.Id;
        }
    }
}