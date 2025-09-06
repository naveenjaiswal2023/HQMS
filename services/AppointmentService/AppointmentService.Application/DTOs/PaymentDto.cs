namespace AppointmentService.Application.DTOs
{
    public record PaymentDto(
        Guid Id,
        Guid AppointmentId,
        decimal Amount,
        string Status,
        DateTime CreatedAt
    );
}
