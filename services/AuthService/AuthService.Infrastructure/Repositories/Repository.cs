using HQMS.QueueService.Infrastructure.Persistence;
using HQMS.QueueService.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HQMS.QueueService.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AuthDbContext _context;

        public Repository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(Guid id) => await _context.Set<T>().FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await Task.CompletedTask; // or handle SaveChanges in UnitOfWork
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await Task.CompletedTask; // same as above
        }
    }

}
