
namespace AuthService.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork, IDisposable
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
