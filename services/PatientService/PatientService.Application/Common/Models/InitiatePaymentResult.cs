using System;

namespace PatientService.Application.Common.Models
{
    public class InitiatePaymentResult
    {
        public Guid PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string PaymentUrl { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
