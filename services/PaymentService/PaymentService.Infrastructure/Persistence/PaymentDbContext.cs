using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Common;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Emit;

namespace PaymentService.Infrastructure.Persistence
{
    public class PaymentDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IDomainEventPublisher _eventPublisher;

        public PaymentDbContext(
            DbContextOptions<PaymentDbContext> options,
            ICurrentUserService currentUser,
            IDomainEventPublisher eventPublisher)
            : base(options)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public DbConnection GetConnection() => Database.GetDbConnection();

        // Entities
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RegistrationFee> RegistrationFees { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.TransactionId).IsRequired().HasMaxLength(50);
                entity.HasIndex(p => p.TransactionId).IsUnique();
                entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Currency).IsRequired().HasMaxLength(3);
                entity.Property(p => p.PaymentMethod).HasConversion<string>();
                entity.Property(p => p.Status).HasConversion<string>();
                entity.Property(p => p.RegistrationFeeType).HasMaxLength(50);
                entity.Property(p => p.PaymentGatewayResponse).HasMaxLength(2000);
                entity.Property(p => p.FailureReason).HasMaxLength(500);
            });

            modelBuilder.Entity<RegistrationFee>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.FeeType).IsRequired().HasMaxLength(50);
                entity.HasIndex(r => r.FeeType).IsUnique();
                entity.Property(r => r.Amount).HasColumnType("decimal(18,2)");
                entity.Property(r => r.Currency).IsRequired().HasMaxLength(3);
                entity.Property(r => r.Description).HasMaxLength(200);
            });

            // Seed data
            //modelBuilder.Entity<RegistrationFee>().HasData(
            //    new RegistrationFee("NEW_PATIENT", 500.00m, "INR", "New Patient Registration Fee") { Id = Guid.NewGuid() },
            //    new RegistrationFee("EMERGENCY", 1000.00m, "INR", "Emergency Registration Fee") { Id = Guid.NewGuid() },
            //    new RegistrationFee("CONSULTATION", 300.00m, "INR", "Consultation Fee") { Id = Guid.NewGuid() }
            //);
            modelBuilder.Entity<RegistrationFee>().HasData(
             new
             {
                 Id = Guid.NewGuid(),
                 FeeType = "NEW_PATIENT",
                 Amount = 500.00m,
                 Currency = "INR",
                 Description = "New Patient Registration Fee",
                 CreatedAt = DateTime.UtcNow,
                 IsActive = true
             },
             new
             {
                 Id = Guid.NewGuid(),
                 FeeType = "EMERGENCY",
                 Amount = 1000.00m,
                 Currency = "INR",
                 Description = "Emergency Registration Fee",
                 CreatedAt = DateTime.UtcNow,
                 IsActive = true
             },
             new
             {
                 Id = Guid.NewGuid(),
                 FeeType = "CONSULTATION",
                 Amount = 300.00m,
                 Currency = "INR",
                 Description = "Consultation Fee",
                 CreatedAt = DateTime.UtcNow,
                 IsActive = true
             }
         );

            // Ignore base domain event to avoid EF tracking issues
            modelBuilder.Ignore<BaseDomainEvent>();

            // Apply Fluent API configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentDbContext).Assembly);
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
