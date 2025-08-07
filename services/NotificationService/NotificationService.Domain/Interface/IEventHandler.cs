using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Interface
{
    public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
    }

}
