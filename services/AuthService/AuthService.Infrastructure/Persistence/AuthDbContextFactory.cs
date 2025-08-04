using AuthService.Domain.Interfaces;
using AuthService.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infrastructure.Persistence
{
    public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("AuthDbConnectionString"));

            var fakeUserContext = new DummyCurrentUserService();
            var fakeEventPublisher = new DummyDomainEventPublisher();

            return new AuthDbContext(optionsBuilder.Options, fakeUserContext, fakeEventPublisher);
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