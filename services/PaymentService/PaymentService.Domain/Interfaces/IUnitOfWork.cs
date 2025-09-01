using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentService.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveAsync(CancellationToken cancellationToken = default);

        IPaymentRepository Payments { get; }
        IRegistrationFeeRepository RegistrationFees { get; }

        // Optional: Transaction support
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
