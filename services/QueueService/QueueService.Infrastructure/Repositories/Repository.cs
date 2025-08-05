using Microsoft.EntityFrameworkCore;
using QueueService.Domain.Interfaces;
using QueueService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly QueueDbContext _context;

        public Repository(QueueDbContext context)
        {
            _context = context;
        }

        {
        }

        {
        }

        {
        }

        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }
    }
}
