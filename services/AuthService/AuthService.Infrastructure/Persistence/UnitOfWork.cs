using AuthService.Domain.Interfaces;

namespace AuthService.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuthDbContext _context;
        

        public UnitOfWork(AuthDbContext context)
        {
            _context = context;
            
        }

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();

    }
}
