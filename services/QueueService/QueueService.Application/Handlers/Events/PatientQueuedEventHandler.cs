using QueueService.Domain.Events;
using QueueService.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService.Application.Handlers.Events
{
    public class PatientQueuedEventHandler : INotificationHandler<PatientQueuedEvent>
    {
        private readonly IAzureServiceBusPublisher _publisher;

        public PatientQueuedEventHandler(IAzureServiceBusPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task Handle(PatientQueuedEvent notification, CancellationToken cancellationToken)
        {
            await _publisher.PublishAsync(notification, cancellationToken);
        }
    }
}