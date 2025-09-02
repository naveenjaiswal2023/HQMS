using MediatR;
using PaymentService.Domain.Enums;
using PaymentService.Application.Common.Models; // Assuming InitiatePaymentResult lives here

namespace PaymentService.Application.Commands
{
    public class InitiateRegistrationPaymentCommand : IRequest<Result<InitiatePaymentResult>>
    {
        public Guid PatientId { get; set; }
        public string FeeType { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? PayerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }

        // Parameterless constructor (required for model binding in ASP.NET Core)
        public InitiateRegistrationPaymentCommand() { }

        // Full constructor
        public InitiateRegistrationPaymentCommand(
            Guid patientId,
            string feeType,
            decimal amount,
            string currency,
            PaymentMethod paymentMethod,
            string? payerName,
            string? customerEmail,
            string? customerPhone)
        {
            PatientId = patientId;
            FeeType = feeType;
            Amount = amount;
            Currency = currency;
            PaymentMethod = paymentMethod;
            PayerName = payerName;
            CustomerEmail = customerEmail;
            CustomerPhone = customerPhone;
        }
    }
}
