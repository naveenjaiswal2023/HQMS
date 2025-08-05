using QueueService.Domain.Interfaces;

namespace QueueService.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly QueueDbContext _context;
        private readonly IQueueItemRepository _queueItemRepository;
        
        public UnitOfWork(QueueDbContext context, IQueueItemRepository queueItemRepository)
        {
            _context = context;
        }


        public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync();
        }
    }
}
