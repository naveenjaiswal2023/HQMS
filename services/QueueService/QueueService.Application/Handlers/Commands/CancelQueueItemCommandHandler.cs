using MediatR;
using QueueService.Application.Commands;
using QueueService.Application.Common.Models;
using QueueService.Application.Exceptions;
using QueueService.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService.Application.Handlers.Commands
{
    public class CancelQueueItemCommandHandler : IRequestHandler<CancelQueueItemCommand, Result<Guid>>
    {
        private readonly IQueueItemRepository _queueItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelQueueItemCommandHandler(IQueueItemRepository queueItemRepository, IUnitOfWork unitOfWork)
        {
            _queueItemRepository = queueItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CancelQueueItemCommand request, CancellationToken cancellationToken)
        {
            var queueItem = await _queueItemRepository.GetByIdAsync(request.QueueItemId, cancellationToken);
            if (queueItem == null)
                return Result<Guid>.Failure($"QueueItem with Id {request.QueueItemId} not found.");

            queueItem.Cancel();

            await _unitOfWork.SaveAsync(cancellationToken);

            return Result<Guid>.Success(queueItem.Id);
        }
    }
}
