using AuthService.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AuthDbContext _context;

        public UnitOfWork(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            // ✅ Let AuthDbContext handle auditing + domain events
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose() => _context.Dispose();
    }
}
