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

        /// <summary>
        /// Retrieves queue items for the current day, filtered by hospital, department, and optionally doctor.
        /// </summary>
        /// <param name="hospitalId">The ID of the hospital.</param>
        /// <param name="departmentId">The ID of the department.</param>
        /// <param name="doctorId">Optional doctor ID to filter by.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of filtered and ordered queue items.</returns>
        Task<IEnumerable<QueueItem>> GetFilteredQueueItemsAsync(
            Guid hospitalId,
            Guid departmentId,
            Guid? doctorId = null,
            CancellationToken cancellationToken = default);
    }
}
