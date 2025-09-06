using Microsoft.EntityFrameworkCore;
using AppointmentService.Domain.Common;
using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Interfaces;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Emit;

namespace AppointmentService.Infrastructure.Persistence
{
    public class AppointmentDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IDomainEventPublisher _eventPublisher;

        public AppointmentDbContext(
            DbContextOptions<AppointmentDbContext> options,
            ICurrentUserService currentUser,
            IDomainEventPublisher eventPublisher)
            : base(options)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public DbConnection GetConnection() => Database.GetDbConnection();

        // Entities
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Appointment>(builder =>
            {
                //builder.OwnsOne(q => q.DoctorInfo);
                //builder.OwnsOne(q => q.PatientInfo);
            });

            // Ignore base domain event to avoid EF tracking issues
            modelBuilder.Ignore<BaseDomainEvent>();

            // Apply Fluent API configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppointmentDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();

            // Collect domain events
            var domainEvents = ChangeTracker
                .Entries<BaseEntity>()
                .SelectMany(e => e.Entity.DomainEvents)
                .Where(e => e != null)
                .ToList();

            // Save changes to database
            var result = await base.SaveChangesAsync(cancellationToken);

            // Clear domain events
            foreach (var entity in ChangeTracker.Entries<BaseEntity>())
            {
                entity.Entity.ClearDomainEvents();
            }

            // Publish domain events (sync or to outbox)
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
            var user = _currentUser?.UserName ?? "System";

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
