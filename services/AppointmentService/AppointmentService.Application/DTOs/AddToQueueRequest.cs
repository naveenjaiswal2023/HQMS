namespace AppointmentService.Application.DTOs
{
    public record AddToQueueRequest(
        Guid AppointmentId,
        Guid PatientId,
        Guid DoctorId,
        string QueueType,
        int Priority = 1
    );
}
