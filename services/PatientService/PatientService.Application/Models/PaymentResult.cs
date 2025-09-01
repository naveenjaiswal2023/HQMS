using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Models
{
    public class PaymentResult
    {
        public Guid PaymentId { get; set; }
        public string? TransactionId { get; set; }
        public string? PaymentUrl { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
    }
}
