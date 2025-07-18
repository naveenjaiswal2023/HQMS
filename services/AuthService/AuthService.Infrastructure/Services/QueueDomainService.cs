using HQMS.QueueService.Domain.Entities;
using HQMS.QueueService.Domain.Enums;
using HQMS.QueueService.Domain.Interfaces;

namespace HQMS.QueueService.Infrastructure.Services
{
    public class QueueDomainService : IQueueDomainService
    {
        public Task<bool> ProcessQueueItemAsCalledAsync(QueueItem item)
        {
            if (item.Status != QueueStatus.Waiting)
                return Task.FromResult(false); // Only Waiting items can be marked as Called

            item.MarkAsCalled(); // Sets status, timestamp, and raises domain event
            return Task.FromResult(true);
        }

        public Task<bool> ProcessQueueItemAsCompletedAsync(QueueItem item)
        {
            if (item.Status != QueueStatus.Called)
                return Task.FromResult(false); // Only Called items can be marked as Completed

            item.MarkAsCompleted(); // Sets status and timestamp
            return Task.FromResult(true);
        }
    }
}
