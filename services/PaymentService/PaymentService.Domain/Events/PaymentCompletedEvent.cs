using PaymentService.Domain.Common;
using PaymentService.Domain.Interfaces;
using System;

namespace PaymentService.Domain.Events
{
    public class PaymentCompletedEvent : BaseDomainEvent, IDomainEvent
    {
        public Guid PaymentId { get; }
        public Guid PatientId { get; }
        public decimal Amount { get; }
        public string Currency { get; }
        public string PaymentGatewayTransactionId { get; }
        public string ReceiptNumber { get; }
        public string InvoiceNumber { get; }

        public PaymentCompletedEvent(
            Guid paymentId,
            Guid patientId,
            decimal amount,
            string currency,
            string paymentGatewayTransactionId,
            string receiptNumber,
            string invoiceNumber)
        {
            PaymentId = paymentId;
            PatientId = patientId;
            Amount = amount;
            Currency = currency;
            PaymentGatewayTransactionId = paymentGatewayTransactionId;
            ReceiptNumber = receiptNumber;
            InvoiceNumber = invoiceNumber;
        }
    }
}
