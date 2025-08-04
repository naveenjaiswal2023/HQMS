using Microsoft.EntityFrameworkCore;
using QueueService.Domain.Entities;
using QueueService.Domain.Interfaces;
using QueueService.Infrastructure.Persistence;

public class QueueItemRepository : IQueueItemRepository
{
    private readonly QueueDbContext _context;

    public QueueItemRepository(QueueDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddAsync(QueueItem item, CancellationToken cancellationToken = default)
    {
        await _context.QueueItems.AddAsync(item, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return item.Id;
    }

    public async Task<QueueItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.QueueItems
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    public async Task<QueueItem?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        return await _context.QueueItems
            .FirstOrDefaultAsync(q => q.AppointmentId == appointmentId, cancellationToken);
    }

    public async Task<IEnumerable<QueueItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.QueueItems.ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(QueueItem entity, CancellationToken cancellationToken = default)
    {
        _context.QueueItems.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(QueueItem entity, CancellationToken cancellationToken = default)
    {
        _context.QueueItems.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetNextPositionAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        var lastItem = await _context.QueueItems
            .Where(q => q.DoctorId == doctorId)
            .OrderByDescending(q => q.Position)
            .FirstOrDefaultAsync(cancellationToken);

        return lastItem?.Position + 1 ?? 1;
    }

    Task IRepository<QueueItem>.AddAsync(QueueItem entity, CancellationToken cancellationToken)
    {
        return AddAsync(entity, cancellationToken);
    }
}