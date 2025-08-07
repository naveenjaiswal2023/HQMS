using QueueService.Domain.Entities;
using QueueService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Domain.Interfaces
{
    public interface IQueueItemRepository: IRepository<QueueItem>
    {
        Task<int> GetNextPositionAsync(Guid doctorId, CancellationToken cancellationToken = default);
        Task<QueueItem?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    }
}
