using AuthService.Application.Interfaces;
using AuthService.Domain.Common;
using AuthService.Domain.Identity;
using AuthService.Domain.Interfaces;
using HQMS.QueueService.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace AuthService.Infrastructure.Persistence
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>, IAuthDbContext
    {
        private readonly ICurrentUserService _userContext;
        private readonly IDomainEventPublisher _eventPublisher;

        public AuthDbContext(
            DbContextOptions<AuthDbContext> options,
            ICurrentUserService userContext,
            IDomainEventPublisher eventPublisher)
            : base(options)
        {
            _userContext = userContext;
            _eventPublisher = eventPublisher;
        }

        public DbConnection GetConnection() => Database.GetDbConnection();
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        // ✅ Expose Attach via IAuthDbContext
        public void Attach<TEntity>(TEntity entity) where TEntity : class
        {
            base.Attach(entity);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();

            var domainEvents = ChangeTracker.Entries<IBaseEntity>()
                .SelectMany(e => e.Entity.DomainEvents)
                .Where(e => e != null)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            foreach (var entity in ChangeTracker.Entries<IBaseEntity>())
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
