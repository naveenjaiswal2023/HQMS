using Azure.Messaging.ServiceBus;
using System;
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

        public async Task PublishAsync<TEvent>(TEvent @event)
        {
            var messageBody = JsonSerializer.Serialize(@event);
            var message = new ServiceBusMessage(messageBody)
            {
                ContentType = "application/json",
                Subject = typeof(TEvent).Name // Subject can be used to identify event type
            };

            await _sender.SendMessageAsync(message);
        }
    }
}
