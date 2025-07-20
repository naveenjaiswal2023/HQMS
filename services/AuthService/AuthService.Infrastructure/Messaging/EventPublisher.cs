using AuthService.Domain.Common;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Messaging
{
    internal class EventPublisher : BaseDomainEvent
    {
        private readonly ServiceBusSender _sender;

        public EventPublisher(ServiceBusClient client, IConfiguration config)
        {
            _sender = client.CreateSender(config["ServiceBus:Topic"]);
        }

        public async Task PublishAsync<T>(T @event) where T : class
        {
            var message = new ServiceBusMessage(JsonConvert.SerializeObject(@event));
            message.ApplicationProperties.Add("Type", typeof(T).Name);
            await _sender.SendMessageAsync(message);
        }
    }
}
