using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Infrastructure.Models
{
    public class RazorpayConfig
    {
        public string KeyId { get; set; }
        public string KeySecret { get; set; }
        public string WebhookSecret { get; set; }
    }
}
