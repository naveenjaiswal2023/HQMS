using MediatR;
using QueueService.Application.Commands;
using QueueService.Application.Exceptions;
using QueueService.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService.Application.Handlers.Commands
{
    public class MarkQueueItemAsSkippedCommandHandler : IRequestHandler<MarkQueueItemAsSkippedCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueueItemRepository _queueRepository;

        public MarkQueueItemAsSkippedCommandHandler(IUnitOfWork unitOfWork, IQueueItemRepository queueRepository)
        {
            _unitOfWork = unitOfWork;
            _queueRepository = queueRepository;
        }

        public async Task<Guid> Handle(MarkQueueItemAsSkippedCommand request, CancellationToken cancellationToken)
        {
            var queueItem = await _queueRepository.GetByIdAsync(request.QueueItemId, cancellationToken);
            if (queueItem == null)
                throw new NotFoundException("QueueItem", request.QueueItemId);

            queueItem.MarkAsSkipped();

            await _unitOfWork.SaveAsync(cancellationToken);

            return queueItem.Id;
        }
    }
}
