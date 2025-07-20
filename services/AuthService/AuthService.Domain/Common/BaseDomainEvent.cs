
using AuthService.Domain.Interfaces;
using MediatR;

namespace AuthService.Domain.Common
{
    public abstract class BaseDomainEvent : INotification, IDomainEvent
    {
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
        public Guid EventId { get; protected set; } = Guid.NewGuid();
    }
}
