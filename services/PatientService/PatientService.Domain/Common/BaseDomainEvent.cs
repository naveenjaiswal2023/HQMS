
using PatientService.Domain.Interfaces;
using MediatR;

namespace PatientService.Domain.Common
{
    public abstract class BaseDomainEvent : IDomainEvent
    {
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
        public Guid EventId { get; protected set; } = Guid.NewGuid();
    }
}
