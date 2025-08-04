
using AuthService.Domain.Interfaces;
using MediatR;

namespace AuthService.Domain.Common
{
    public abstract class BaseDomainEvent : IDomainEvent, INotification
    {
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
        public Guid EventId { get; protected set; } = Guid.NewGuid();
    }
}
