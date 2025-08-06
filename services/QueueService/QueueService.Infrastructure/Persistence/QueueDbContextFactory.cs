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
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../QueueService.API");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables() // ✅ Add this
                .Build();

            // ✅ Prefer environment variable
            var connectionString = Environment.GetEnvironmentVariable("QueueDbConnectionString")
                ?? configuration.GetConnectionString("QueueDbConnectionString");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Database connection string not found.");

            var optionsBuilder = new DbContextOptionsBuilder<QueueDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var fakeUserContext = new DummyCurrentUserService();
            var fakeEventPublisher = new DummyDomainEventPublisher();

            return new QueueDbContext(optionsBuilder.Options, fakeUserContext, fakeEventPublisher);
        }

        private class DummyCurrentUserService : ICurrentUserService
        {
            public string? UserId => "migration-user";
            public string? UserName => "migration-user";
        }

        private class DummyDomainEventPublisher : IDomainEventPublisher
        {
            public Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default)
                where T : IDomainEvent
            {
                return Task.CompletedTask;
            }
        }
    }
}
