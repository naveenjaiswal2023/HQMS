using PaymentService.Domain.Common;
using PaymentService.Domain.Interfaces;
using System;

namespace PaymentService.Domain.Events
{
    public class PaymentFailedEvent : BaseDomainEvent, IDomainEvent
    {
        public Guid PaymentId { get; }
        public Guid PatientId { get; }
        public decimal Amount { get; }
        public string Currency { get; }
        public string FailureReason { get; }

        public PaymentFailedEvent(
            Guid paymentId,
            Guid patientId,
            decimal amount,
            string currency,
            string failureReason)
        {
            PaymentId = paymentId;
            PatientId = patientId;
            Amount = amount;
            Currency = currency;
            FailureReason = failureReason;
        }
    }
}