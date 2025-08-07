using System;
using System.Collections.Generic;

namespace AuthService.Domain.Interfaces
{
    public interface IBaseEntity
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        string? CreatedBy { get; set; }
        string? UpdatedBy { get; set; }
        bool IsDeleted { get; set; }
        byte[] RowVersion { get; set; }

        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

        void AddDomainEvent(IDomainEvent domainEvent);
        void RemoveDomainEvent(IDomainEvent domainEvent);
        void ClearDomainEvents();
    }
}
