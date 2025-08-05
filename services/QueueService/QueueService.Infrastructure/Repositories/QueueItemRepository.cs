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

    public async Task<Guid> AddAsync(QueueItem item, CancellationToken cancellationToken = default)
        {
            return item.Id;
        }

        {
        }

        {
        }

        {
        }

        {
        }

        {
            // Assuming you have a method to get the next position based on the doctor's queue
            var lastItem = await _context.QueueItems
                .Where(q => q.DoctorId == doctorId)
                .OrderByDescending(q => q.Position)

        }

        {
    }
}
