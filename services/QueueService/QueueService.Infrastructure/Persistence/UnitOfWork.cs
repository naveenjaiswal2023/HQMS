using QueueService.Domain.Interfaces;

namespace QueueService.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly QueueDbContext _context;
        

        public UnitOfWork(QueueDbContext context)
        {
            _context = context;
            
        }

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();

    }
}
