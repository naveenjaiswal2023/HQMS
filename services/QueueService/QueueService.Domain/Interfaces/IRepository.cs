using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
    }
}
