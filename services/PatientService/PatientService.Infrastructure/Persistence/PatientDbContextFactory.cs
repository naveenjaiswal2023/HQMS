using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PatientService.Domain.Interfaces;
using PatientService.Infrastructure.Persistence;
using System.IO;

namespace QueueService.Infrastructure.Persistence
{
    public class PatientDbContextFactory : IDesignTimeDbContextFactory<PatientDbContext>
    {
        public PatientDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../PatientService.API");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables() // ✅ Add this
                .Build();

            // ✅ Prefer environment variable
            var connectionString = Environment.GetEnvironmentVariable("PatientDbConnectionString")
                ?? configuration.GetConnectionString("PatientDbConnectionString");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Database connection string not found.");

            var optionsBuilder = new DbContextOptionsBuilder<PatientDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var fakeUserContext = new DummyCurrentUserService();
            var fakeEventPublisher = new DummyDomainEventPublisher();

            return new PatientDbContext(optionsBuilder.Options, fakeUserContext, fakeEventPublisher);
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
