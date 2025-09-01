using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using PaymentService.Domain.Interfaces;

namespace PaymentService.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PaymentDbContext _context;
        private IDbContextTransaction _transaction;

        public UnitOfWork(PaymentDbContext context, IPaymentRepository paymentRepository, IRegistrationFeeRepository registrationFeeRepository)
        {
            _context = context;
            Payments = paymentRepository;
            RegistrationFees = registrationFeeRepository;
        }

        public IPaymentRepository Payments { get; }
        public IRegistrationFeeRepository RegistrationFees { get; }

        // Save changes
        public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        // Transaction support
        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _transaction?.CommitAsync();
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _transaction?.RollbackAsync();
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        private async Task DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        // Dispose DbContext
        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}