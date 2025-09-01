using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using PatientService.Domain.Interfaces;

namespace PatientService.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PatientDbContext _context;
        private readonly IPatientRepository _patientRepository;
        private IDbContextTransaction _transaction;

        public UnitOfWork(PatientDbContext context, IPatientRepository patientRepository)
        {
            _context = context;
            _patientRepository = patientRepository;
        }

        // Expose repository
        public IPatientRepository PatientRepository => _patientRepository;

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