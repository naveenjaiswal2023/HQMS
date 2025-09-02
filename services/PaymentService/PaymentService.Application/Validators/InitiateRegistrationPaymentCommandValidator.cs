using FluentValidation;
using PaymentService.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Validators
{
    public class InitiateRegistrationPaymentCommandValidator : AbstractValidator<InitiateRegistrationPaymentCommand>
    {
        public InitiateRegistrationPaymentCommandValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("Patient ID is required");

            RuleFor(x => x.FeeType)
                .NotEmpty().WithMessage("Fee type is required")
                .Must(BeValidFeeType).WithMessage("Invalid fee type");

            RuleFor(x => x.CustomerEmail)
                .NotEmpty().WithMessage("Customer email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.CustomerPhone)
                .NotEmpty().WithMessage("Customer phone is required")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format");
        }

        private bool BeValidFeeType(string feeType)
        {
            var validTypes = new[] { "NEW_PATIENT", "CONSULTATION", "EMERGENCY" };
            return validTypes.Contains(feeType);
        }
    }
}
