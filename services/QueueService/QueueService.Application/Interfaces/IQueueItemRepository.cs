using QueueService.Domain.Entities;
using QueueService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.Interfaces
{
    public interface IQueueItemRepository: IRepository<QueueItem>
    {
        Task<int> GetNextPositionAsync(Guid doctorId);
        Task<QueueItem?> GetByAppointmentIdAsync(Guid appointmentId);
    }
}
