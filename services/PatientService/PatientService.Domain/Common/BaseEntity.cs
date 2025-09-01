using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientService.Domain.Common
{
    public abstract class BaseEntity
    {
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // RowVersion for concurrency control
        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;

        private readonly List<BaseDomainEvent> _domainEvents = new();

        [NotMapped]
        public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(BaseDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(BaseDomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
