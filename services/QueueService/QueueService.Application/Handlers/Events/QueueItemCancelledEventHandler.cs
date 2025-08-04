using QueueService.Domain.Events;
using QueueService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.Handlers.Events
{
    public class QueueItemCancelledEventHandler
    {
        private readonly IAzureServiceBusPublisher _publisher;

        public QueueItemCancelledEventHandler(IAzureServiceBusPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task HandleAsync(QueueItemCancelledEvent @event)
        {
            await _publisher.PublishAsync(@event);
        }
    }
}
