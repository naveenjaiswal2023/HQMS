using PaymentService.Domain.Models.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Interfaces
{
    public interface IPaymentGateway
    {
        Task<PaymentInitiationResponse> InitiatePaymentAsync(PaymentInitiationRequest request);
        Task<PaymentVerificationResponse> VerifyPaymentAsync(string transactionId);
        Task<RefundResponse> ProcessRefundAsync(RefundRequest request);
    }
}
