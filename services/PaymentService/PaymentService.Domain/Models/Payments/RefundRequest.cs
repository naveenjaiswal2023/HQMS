using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Domain.Models.Payments
{
    public class RefundRequest
    {
        public string OriginalTransactionId { get; set; }
        public decimal RefundAmount { get; set; }
        public string Reason { get; set; }

    }
}
