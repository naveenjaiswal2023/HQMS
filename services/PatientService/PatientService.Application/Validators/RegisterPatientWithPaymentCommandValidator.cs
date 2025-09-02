using FluentValidation;
using PatientService.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Validators
{
    public class RegisterPatientWithPaymentCommandValidator : AbstractValidator<RegisterPatientWithPaymentCommand>
    {
        public RegisterPatientWithPaymentCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(200).WithMessage("Email cannot exceed 200 characters");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past")
                .GreaterThan(DateTime.Today.AddYears(-120)).WithMessage("Invalid date of birth");

            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage("Invalid payment method");
        }
    }
}
