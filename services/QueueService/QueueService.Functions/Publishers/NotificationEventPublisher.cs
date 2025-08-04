using Azure.Messaging.ServiceBus;
using QueueService.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QueueService.Functions.Publishers
{
    public class NotificationEventPublisher
    {
        private readonly ServiceBusSender _sender;

        public NotificationEventPublisher(ServiceBusClient client)
        {
            _sender = client.CreateSender("notification.events.topic");
        }

        public async Task PublishAsync(QueueItemCalledEvent @event)
        {
            var messageBody = JsonSerializer.Serialize(@event);
            var message = new ServiceBusMessage(messageBody)
            {
                ContentType = "application/json"
            };

            await _sender.SendMessageAsync(message);
        }
    }
}
