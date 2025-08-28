using AuthService.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Domain.Identity
{
    public class ApplicationUser : IdentityUser, IBaseEntity
    {
        // 👤 Domain-specific fields
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; } = true;

        // 🕒 Audit fields (from IBaseEntity)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        // 📌 Domain Events
        private readonly List<IDomainEvent> _domainEvents = new();

        [NotMapped] // EF should NOT persist this list
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            if (domainEvent == null) throw new ArgumentNullException(nameof(domainEvent));
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            if (domainEvent == null) return;
            _domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
