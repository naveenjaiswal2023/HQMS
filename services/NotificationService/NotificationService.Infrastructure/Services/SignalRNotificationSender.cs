using Microsoft.AspNetCore.SignalR;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Events;
using NotificationService.SignalR.Hubs;

namespace NotificationService.Infrastructure.Services;

public class SignalRNotificationSender : INotificationSender
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationSender(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendQueueCalledNotificationAsync(QueueItemCalledEvent evt)
    {
        var message = new
        {
            QueueItemId = evt.QueueItemId,
            PatientId = evt.PatientId,
            CalledAt = evt.CalledAt
        };

        // Send to doctor (or patient, or both)
        await _hubContext.Clients.User(evt.DoctorId)
            .SendAsync("ReceiveNotification", message);
    }
}
