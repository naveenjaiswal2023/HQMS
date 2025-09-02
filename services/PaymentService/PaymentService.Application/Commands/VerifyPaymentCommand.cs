using MediatR;
using PaymentService.Application.Common.Models;

namespace PaymentService.Application.Commands
{
    public class VerifyPaymentCommand : IRequest<Result<PaymentVerificationResult>>
    {
        public string TransactionId { get; set; }

        // ✅ Parameterless constructor (needed for model binding / deserialization)
        public VerifyPaymentCommand() { }

        // ✅ Full constructor
        public VerifyPaymentCommand(string transactionId)
        {
            TransactionId = transactionId;
        }
    }
}
