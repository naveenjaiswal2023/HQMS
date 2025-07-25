using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Infrastructure.Persistence
{
    public class QueueDbContextFactory : IDesignTimeDbContextFactory<QueueDbContext>
    {
        public QueueDbContext CreateDbContext(string[] args)
        {
            var configPath = Path.Combine(Directory.GetCurrentDirectory(), "services", "QueueService", "QueueService.API");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(configPath)
                .AddJsonFile("appsettings.Development.json")
                .Build();

            var connectionString = configuration.GetConnectionString("QueueDbConnectionString");

            var optionsBuilder = new DbContextOptionsBuilder<QueueDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // 👇 Pass nulls for ICurrentUserService and IDomainEventPublisher for design-time only
            return new QueueDbContext(optionsBuilder.Options, null!, null!);
        }
    }
}
