using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Domain.Payments
{
    public class PaymentVerificationResult
    {
        public bool Verified { get; set; }
        public string PaymentId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
