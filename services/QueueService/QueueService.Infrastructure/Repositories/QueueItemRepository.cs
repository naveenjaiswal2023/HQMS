using Microsoft.EntityFrameworkCore;
using QueueService.Application.Common.Interfaces;
using QueueService.Application.Interfaces;
using QueueService.Domain.Entities;
using QueueService.Domain.Interfaces;
using QueueService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Infrastructure.Repositories
{
    public class QueueItemRepository : IQueueItemRepository
    {
        private readonly QueueDbContext _context;
        public QueueItemRepository(QueueDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddAsync(QueueItem item)
        {
            _context.QueueItems.Add(item);
            await _context.SaveChangesAsync();
            return item.Id;
        }

        public Task DeleteAsync(QueueItem entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<QueueItem>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<QueueItem?> GetByAppointmentIdAsync(Guid appointmentId)
        {
            throw new NotImplementedException();
        }

        public Task<QueueItem> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetNextPositionAsync(Guid doctorId)
        {
            // Assuming you have a method to get the next position based on the doctor's queue
            var lastItem = await _context.QueueItems
                .Where(q => q.DoctorId == doctorId)
                .OrderByDescending(q => q.Position)
                .FirstOrDefaultAsync();
            return lastItem?.Position + 1 ?? 1; // If no items, start at position 1
        }

        public Task UpdateAsync(QueueItem entity)
        {
            throw new NotImplementedException();
        }

        Task IRepository<QueueItem>.AddAsync(QueueItem entity)
        {
            return AddAsync(entity);
        }
    }
}
