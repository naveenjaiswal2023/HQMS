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

    public async Task SendNotificationAsync<T>(T evt) where T : class
    {
        if (evt is QueueItemCalledEvent calledEvent)
        {
            if (string.IsNullOrEmpty(calledEvent.DoctorName))
                throw new ArgumentException("DoctorId cannot be null or empty");

            await _hubContext.Clients.User(calledEvent.DoctorName)
                .SendAsync("ReceiveNotification", new
                {
                    Type = "QueueItemCalled",
                    calledEvent.QueueItemId,
                    calledEvent.PatientId,
                    calledEvent.CalledAt
                });
        }
        else if (evt is QueueItemCompletedEvent completedEvent)
        {
            await _hubContext.Clients.User(completedEvent.DoctorId.ToString())
                .SendAsync("ReceiveNotification", new
                {
                    Type = "QueueItemCompleted",
                    completedEvent.QueueItemId,
                    completedEvent.CompletedAt
                });
        }
        else if (evt is QueueCompletedEvent queueCompletedEvent)
        {
            await _hubContext.Clients.User(queueCompletedEvent.DoctorId.ToString())
                .SendAsync("ReceiveNotification", new
                {
                    Type = "QueueCompleted",
                    queueCompletedEvent.QueueId,
                    queueCompletedEvent.CompletedAt,
                    Message = $"Queue completed for Dr. {queueCompletedEvent.DoctorName}"
                });
        }
        else if (evt is QueueItemCancelledEvent cancelled)
        {
            await _hubContext.Clients.User(cancelled.DoctorId.ToString())
                .SendAsync("ReceiveNotification", new
                {
                    Type = "QueueItemCancelled",
                    cancelled.QueueItemId,
                    cancelled.CancelledBy,
                    cancelled.CancelledAt,
                    Message = $"Queue item was cancelled by {cancelled.CancelledBy}."
                });
        }
        else if (evt is QueueItemSkippedEvent skipped)
        {
            await _hubContext.Clients.User(skipped.DoctorId.ToString())
                .SendAsync("ReceiveNotification", new
                {
                    Type = "QueueItemSkipped",
                    skipped.QueueItemId,
                    skipped.Reason,
                    skipped.SkippedAt,
                    Message = $"Queue item was skipped. Reason: {skipped.Reason}"
                });
        }
        else
        {
            throw new NotSupportedException($"Notification type {evt.GetType().Name} is not supported.");
        }
    }
}
