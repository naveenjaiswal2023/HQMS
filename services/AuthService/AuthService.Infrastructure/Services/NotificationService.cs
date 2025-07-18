using Azure.Messaging.ServiceBus;
using HQMS.QueueService.Application.Common.Interfaces;
using HQMS.QueueService.Domain.Entities;
using System.Text.Json;

namespace HQMS.QueueService.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusSender _sender;

        public NotificationService(ServiceBusClient serviceBusClient)
        {
            _serviceBusClient = serviceBusClient;
            _sender = _serviceBusClient.CreateSender("queue-notifications");
        }

        public async Task SendQueueJoinedNotificationAsync(QueueItem queueItem)
        {
            var message = new
            {
                EventType = "QueueJoined",
                PatientId = queueItem.PatientId,
                Position = queueItem.Position,
                EstimatedWaitTime = queueItem.EstimatedWaitTime,
                Department = queueItem.Department,
                Timestamp = DateTime.UtcNow
            };

            await SendMessageAsync(message);
        }

        public async Task SendPatientCalledNotificationAsync(QueueItem queueItem)
        {
            var message = new
            {
                EventType = "PatientCalled",
                PatientId = queueItem.PatientId,
                DoctorId = queueItem.DoctorId,
                Department = queueItem.Department,
                Timestamp = DateTime.UtcNow
            };

            await SendMessageAsync(message);
        }

        public async Task SendConsultationCompletedNotificationAsync(QueueItem queueItem)
        {
            var message = new
            {
                EventType = "ConsultationCompleted",
                PatientId = queueItem.PatientId,
                DoctorId = queueItem.DoctorId,
                Department = queueItem.Department,
                Duration = queueItem.CompletedAt - queueItem.CalledAt,
                Timestamp = DateTime.UtcNow
            };

            await SendMessageAsync(message);
        }

        private async Task SendMessageAsync(object message)
        {
            var json = JsonSerializer.Serialize(message);
            var serviceBusMessage = new ServiceBusMessage(json);
            await _sender.SendMessageAsync(serviceBusMessage);
        }
    }
}
