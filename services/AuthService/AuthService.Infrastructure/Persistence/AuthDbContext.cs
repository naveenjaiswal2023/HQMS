
using AuthService.Domain.Common;
using HQMS.QueueService.Domain.Entities;
using HQMS.QueueService.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AuthService.Infrastructure.Persistence
{
    public class AuthDbContext : DbContext
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        public AuthDbContext(
            DbContextOptions<AuthDbContext> options,
            IMediator mediator,
            ICurrentUserService currentUserService) : base(options)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentUser = _currentUserService.UserName ?? "System";

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = currentUser;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedBy = currentUser;
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            await DispatchDomainEventsAsync();

            return result;
        }

        private async Task DispatchDomainEventsAsync()
        {
            var domainEntities = ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.Entity.DomainEvents != null && e.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            domainEntities.ForEach(e => e.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent);
            }
        }
    }
}
