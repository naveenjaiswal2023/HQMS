using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Interfaces
{
    
    public interface IAuthDbContext
    {
        void Attach<TEntity>(TEntity entity) where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

}
