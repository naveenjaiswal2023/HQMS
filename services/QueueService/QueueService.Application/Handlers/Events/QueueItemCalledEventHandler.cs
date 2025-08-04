using QueueService.Domain.Interfaces;
using QueueService.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.Handlers.Events
{
    public class QueueItemCalledEventHandler
    {
        private readonly IAzureServiceBusPublisher _publisher;

        public QueueItemCalledEventHandler(IAzureServiceBusPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task HandleAsync(QueueItemCalledEvent @event)
        {
            await _publisher.PublishAsync(@event);
        }
    }
}
