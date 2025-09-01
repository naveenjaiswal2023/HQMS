using PatientService.Application.Commands;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Interfaces
{
    public interface IPaymentServiceClient
    {
        //Task<InitiatePaymentResult> InitiateRegistrationPaymentAsync(Guid patientId, string feeType, PaymentMethod paymentMethod, string email, string phone);
        // In IPaymentServiceClient
        Task<InitiatePaymentResult> InitiateRegistrationPaymentAsync(
            Guid patientId,
            string registrationFeeType,
            decimal amount,
            PatientService.Domain.Enums.PaymentMethod paymentMethod,
            string payerName,
            string? email,
            string phoneNumber
        );

        Task<PaymentVerificationResult> VerifyPaymentAsync(string transactionId);
    }
}
