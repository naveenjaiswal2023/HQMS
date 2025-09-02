using System;

namespace PaymentService.Application.Commands
{
    public class InitiatePaymentResult
    {
        public Guid PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string PaymentUrl { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public InitiatePaymentResult(Guid paymentId, string transactionId, string paymentUrl, decimal amount, string currency)
        {
            PaymentId = paymentId;
            TransactionId = transactionId;
            PaymentUrl = paymentUrl;
            Amount = amount;
            Currency = currency;
        }
    }
}
