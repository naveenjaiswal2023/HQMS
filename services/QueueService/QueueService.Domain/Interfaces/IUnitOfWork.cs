using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveAsync(CancellationToken cancellationToken = default);
        IQueueItemRepository QueueItems { get; }
        // Expose repositories here if needed:
        // IPatientRepository Patients { get; }
        // IDoctorRepository Doctors { get; }
    }
}
