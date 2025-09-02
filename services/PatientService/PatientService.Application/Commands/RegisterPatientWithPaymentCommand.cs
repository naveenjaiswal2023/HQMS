using MediatR;
using PatientService.Application.Common.Models;
using PatientService.Application.Models;
using PatientService.Domain.Enums;

namespace PatientService.Application.Commands
{
    public class RegisterPatientWithPaymentCommand : IRequest<Result<PatientRegistrationResult>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string Address { get; set; }
        public string? EmergencyContact { get; set; }
        public string? MedicalHistory { get; set; }
        public PaymentMethod PaymentMethod { get; set; } // ✅ always required
        public string RegistrationFeeType { get; set; } = "NEW_PATIENT";

        public RegisterPatientWithPaymentCommand(
            string firstName,
            string lastName,
            DateTime dateOfBirth,
            string gender,
            string phoneNumber,
            string? email,
            string address,
            string? emergencyContact,
            string? medicalHistory,
            PaymentMethod paymentMethod,
            string registrationFeeType = "NEW_PATIENT")
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            PhoneNumber = phoneNumber;
            Email = email;
            Address = address;
            EmergencyContact = emergencyContact;
            MedicalHistory = medicalHistory;
            PaymentMethod = paymentMethod;
            RegistrationFeeType = registrationFeeType;
        }
    }
}
