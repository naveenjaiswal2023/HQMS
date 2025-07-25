using AuthService.Domain.Common;
using AuthService.Domain.Identity;
using AuthService.Domain.Interfaces;
using HQMS.QueueService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data.Common;

namespace AuthService.Infrastructure.Persistence
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser, ApplicationRole,string>
    {
        private readonly ICurrentUserService _userContext;
        private readonly IDomainEventPublisher _eventPublisher;

        public AuthDbContext(
            DbContextOptions<AuthDbContext> options,
            ICurrentUserService userContext,
            IDomainEventPublisher eventPublisher
        )
            : base(options)
        {
            _userContext = userContext;
            _eventPublisher = eventPublisher;
        }

        public DbConnection GetConnection() => Database.GetDbConnection();
        public DbSet<OutboxMessage> OutboxMessages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<BaseDomainEvent>();

            // Configure Identity entities with your custom types
            modelBuilder.Entity<ApplicationUser>();
            modelBuilder.Entity<ApplicationRole>();
            
            // Apply other configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();

            var domainEvents = ChangeTracker.Entries<BaseEntity>()
                .SelectMany(e => e.Entity.DomainEvents)
                .Where(e => e != null)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            foreach (var entity in ChangeTracker.Entries<BaseEntity>())
            {
                entity.Entity.ClearDomainEvents();
            }

            foreach (var domainEvent in domainEvents)
            {
                await _eventPublisher.PublishAsync(domainEvent, cancellationToken);
            }

            return result;
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }

        private void ApplyAuditInfo()
        {
            var user = _userContext.UserName ?? "System";

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = user;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = user;
                }
            }
        }
    }
}
