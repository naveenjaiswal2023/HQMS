

using MediatR;
using PaymentService.Domain.Interfaces;

namespace PaymentService.Domain.Common
{
    public abstract class BaseDomainEvent : IDomainEvent
    {
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
        public Guid EventId { get; protected set; } = Guid.NewGuid();
    }
}
