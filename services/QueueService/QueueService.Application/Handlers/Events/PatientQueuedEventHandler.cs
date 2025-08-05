using QueueService.Domain.Events;
using QueueService.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService.Application.Handlers.Events
{
    public class PatientQueuedEventHandler
    {
        private readonly IAzureServiceBusPublisher _publisher;

        public PatientQueuedEventHandler(IAzureServiceBusPublisher publisher)
        {
            _publisher = publisher;
        }

        {
        }
    }
}