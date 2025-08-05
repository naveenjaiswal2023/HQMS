using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using QueueService.Domain.Interfaces;
using System.IO;

namespace QueueService.Infrastructure.Persistence
{
    public class QueueDbContextFactory : IDesignTimeDbContextFactory<QueueDbContext>
    {
        public QueueDbContext CreateDbContext(string[] args)
        {

            var configuration = new ConfigurationBuilder()
                .Build();


            var optionsBuilder = new DbContextOptionsBuilder<QueueDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

        }
    }
}
