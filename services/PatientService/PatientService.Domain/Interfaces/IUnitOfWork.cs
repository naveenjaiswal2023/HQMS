using System;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveAsync(CancellationToken cancellationToken = default);

        // Strongly typed repository
        IPatientRepository PatientRepository { get; }

        // Optional: Transaction support
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
