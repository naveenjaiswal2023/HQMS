namespace AppointmentService.Application.DTOs
{
    public record InitiatePaymentRequest(
        Guid AppointmentId,
        Guid PatientId,
        decimal Amount,
        string Description
    );
}
