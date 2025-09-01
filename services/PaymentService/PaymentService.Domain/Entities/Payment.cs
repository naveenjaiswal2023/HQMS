using PaymentService.Domain.Common;
using PaymentService.Domain.Enums;
using System;

namespace PaymentService.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public Guid Id { get; private set; }

        // 🔹 Core Payment Details
        public string TransactionId { get; private set; }
        public Guid PatientId { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public PaymentStatus Status { get; private set; }

        // 🔹 Gateway / Transaction Details
        public string? PaymentGatewayTransactionId { get; private set; } // ID from Razorpay/Stripe etc.
        public string? PaymentGatewayResponse { get; private set; }
        public string? FailureReason { get; private set; }
        public DateTime? ProcessedAt { get; private set; }

        // 🔹 Hospital / Fee Info
        public string RegistrationFeeType { get; private set; }  // e.g., "Consultation", "Admission"
        public string? Department { get; private set; }          // Which department (OPD, IPD, Diagnostics)
        public string? DoctorId { get; private set; }            // Optional, if payment is doctor-specific
        public string? InvoiceNumber { get; private set; }       // For finance reconciliation
        public string? ReceiptNumber { get; private set; }       // For patient receipt tracking

        // 🔹 Audit
        public string? PayerName { get; private set; }           // Name of the payer (patient/relative)
        public string? PayerEmail { get; private set; }
        public string? PayerPhone { get; private set; }

        protected Payment() { } // For EF Core

        public Payment(Guid patientId, decimal amount, string currency,
                      PaymentMethod paymentMethod, string registrationFeeType,
                      string? payerName, string? payerPhone, string? payerEmail)
        {
            Id = Guid.NewGuid();
            TransactionId = GenerateTransactionId();
            PatientId = patientId;
            Amount = amount > 0 ? amount : throw new ArgumentException("Amount must be positive");
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
            PaymentMethod = paymentMethod;
            Status = PaymentStatus.Pending;
            RegistrationFeeType = registrationFeeType;

            PayerName = payerName;
            PayerPhone = payerPhone;
            PayerEmail = payerEmail;

            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsProcessing()
        {
            Status = PaymentStatus.Processing;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsSuccess(string gatewayResponse, string? gatewayTransactionId = null, string? receiptNumber = null, string? invoiceNumber = null)
        {
            Status = PaymentStatus.Success;
            PaymentGatewayResponse = gatewayResponse;
            PaymentGatewayTransactionId = gatewayTransactionId;
            ReceiptNumber = receiptNumber ?? GenerateReceiptNumber();
            InvoiceNumber = invoiceNumber ?? GenerateInvoiceNumber();
            ProcessedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsFailed(string failureReason)
        {
            Status = PaymentStatus.Failed;
            FailureReason = failureReason;
            ProcessedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        private string GenerateTransactionId()
        {
            return $"TXN_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..8]}";
        }

        private string GenerateReceiptNumber()
        {
            return $"RCPT_{DateTime.UtcNow:yyyyMMdd}_{Guid.NewGuid().ToString("N")[..6]}";
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV_{DateTime.UtcNow:yyyyMM}_{Guid.NewGuid().ToString("N")[..6]}";
        }
    }
}
