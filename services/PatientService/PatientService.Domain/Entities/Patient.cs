using PatientService.Domain.Common;
using PatientService.Domain.Enums;
using PatientService.Domain.Events;
using Stripe;
using System;

namespace PatientService.Domain.Entities
{
    public class Patient : BaseEntity
    {
        public Guid Id { get; private set; }

        // 🔹 Basic Mandatory Info
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string Gender { get; private set; } // Male/Female/Other
        public string PhoneNumber { get; private set; }

        // 🔹 Optional Contact Info
        public string? MiddleName { get; private set; }
        public string? AlternatePhoneNumber { get; private set; }
        public string? Email { get; private set; }
        public string? Address { get; private set; }
        public string? City { get; private set; }
        public string? State { get; private set; }
        public string? Country { get; private set; }
        public string? ZipCode { get; private set; }

        // 🔹 Optional Emergency & Insurance
        public string? EmergencyContact { get; private set; }
        public string? EmergencyContactRelation { get; private set; }
        public string? EmergencyContactPhone { get; private set; }

        public string? InsuranceProvider { get; private set; }
        public string? InsurancePolicyNumber { get; private set; }

        // 🔹 Optional Medical Info
        public string? BloodGroup { get; private set; }
        public string? Allergies { get; private set; }
        public string? MedicalHistory { get; private set; }
        public string? CurrentMedications { get; private set; }

        // 🔹 Registration & System Tracking
        public PatientRegistrationStatus RegistrationStatus { get; private set; }
        public string UHID { get; private set; } // Unique Hospital Patient Number
        public DateTime? ActivatedAt { get; private set; }

        // 🔹 Payment reference (optional)
        public string? RegistrationPaymentId { get; private set; }

        protected Patient() {}

        public Patient(string firstName, string lastName, DateTime dateOfBirth,
                       string gender, string phoneNumber)
        {
            Id = Guid.NewGuid();
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            DateOfBirth = dateOfBirth;
            Gender = gender ?? throw new ArgumentNullException(nameof(gender));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));

            RegistrationStatus = PatientRegistrationStatus.PendingPayment;
            UHID = GeneratePatientNumber();
            CreatedAt = DateTime.UtcNow;

            AddDomainEvent(new PatientRegisteredEvent(
                this.Id,
                this.UHID,
                $"{this.FirstName} {this.LastName}",
                this.PhoneNumber,
                DateTime.UtcNow
            ));
        }

        // 🔹 Payment handling
        public void LinkPayment(string paymentId)
        {
            RegistrationPaymentId = paymentId ?? throw new ArgumentNullException(nameof(paymentId));
            MarkPaymentProcessing();
        }

        public void MarkPaymentProcessing()
        {
            RegistrationStatus = PatientRegistrationStatus.PaymentProcessing;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ActivateRegistration()
        {
            RegistrationStatus = PatientRegistrationStatus.Active;
            ActivatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new PatientActivatedEvent(Id, UHID));
        }

        public void MarkPaymentFailed(string failureReason)
        {
            RegistrationStatus = PatientRegistrationStatus.PaymentFailed;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new PatientPaymentFailedEvent(Id, UHID, failureReason));
        }

        public void MarkPaymentCompleted()
        {
            RegistrationStatus = PatientRegistrationStatus.Completed;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new PatientPaymentCompletedEvent(Id, UHID, RegistrationPaymentId!));
        }

        private string GeneratePatientNumber()
        {
            return $"P{DateTime.UtcNow:yyyyMM}{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
        }
    }
}
