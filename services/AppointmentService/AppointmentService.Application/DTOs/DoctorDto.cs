namespace AppointmentService.Application.DTOs
{
    public record DoctorDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Specialization,
        Guid HospitalId,
        string Email,
        string Phone
    );
}
