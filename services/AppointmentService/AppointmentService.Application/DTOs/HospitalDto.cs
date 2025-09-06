namespace AppointmentService.Application.DTOs
{
    public record HospitalDto(
        Guid Id,
        string Name,
        string Address,
        string City,
        string Phone,
        string Email
    );
}
