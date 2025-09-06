namespace AppointmentService.Application.DTOs
{
    public record PatientDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string Phone,
        DateTime DateOfBirth
    );
}
