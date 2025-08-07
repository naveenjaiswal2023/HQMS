using AppointmentService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Infrastructure.Persistence
{
    public class AppointmentDbContextFactory : IDesignTimeDbContextFactory<AppointmentDbContext>
    {
        public AppointmentDbContext CreateDbContext(string[] args)
        {
            //var basePath = Path.Combine(Directory.GetCurrentDirectory(), "services", "QueueService", "QueueService.API");
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../AppointmentService.API");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.Development.json")
                .Build();

            var connectionString = configuration.GetConnectionString("AppointmentDbConnectionString");

            var optionsBuilder = new DbContextOptionsBuilder<AppointmentDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var fakeUserContext = new DummyCurrentUserService();
            var fakeEventPublisher = new DummyDomainEventPublisher();

            return new AppointmentDbContext(optionsBuilder.Options, fakeUserContext, fakeEventPublisher);
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
