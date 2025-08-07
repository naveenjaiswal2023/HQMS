using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Domain.Interfaces
{
    public interface IDomainEventPublisher
    {
        Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken) where T : IDomainEvent;
    }

}
