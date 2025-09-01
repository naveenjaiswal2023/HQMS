using MediatR;
using PatientService.Application.Models;
using PatientService.Domain.Enums;

namespace PatientService.Application.Commands
{
    public record RegisterPatientWithPaymentCommand(
        string FirstName,
        string LastName,
        DateTime DateOfBirth,
        string Gender,
        string PhoneNumber,
        string? Email,
        string Address,
        string? EmergencyContact,
        string? MedicalHistory,
        PaymentMethod PaymentMethod,   // ✅ always required
        string RegistrationFeeType = "NEW_PATIENT"
    ) : IRequest<PatientRegistrationResult>;
}
