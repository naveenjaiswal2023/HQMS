using QueueService.Domain.Events;
using QueueService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.Handlers.Events
{
    public class QueueItemSkippedEventHandler
    {
        private readonly IAzureServiceBusPublisher _publisher;

        public QueueItemSkippedEventHandler(IAzureServiceBusPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task HandleAsync(QueueItemSkippedEvent @event)
        {
            await _publisher.PublishAsync(@event);
        }
    }
}
