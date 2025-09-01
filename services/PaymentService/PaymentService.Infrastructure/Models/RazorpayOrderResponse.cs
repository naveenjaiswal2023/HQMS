using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Infrastructure.Models
{
    public class RazorpayOrderResponse
    {
        public string Id { get; set; }
        public string Entity { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public string Receipt { get; set; }
        public string Status { get; set; }
    }
}
