using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Infrastructure.Models
{
    public class RazorpayPaymentsResponse
    {
        public List<RazorpayPaymentItem> Items { get; set; }
    }
}
