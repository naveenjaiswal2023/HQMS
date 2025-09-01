using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PaymentService.Domain.Models.Payments
{
    public class StripeErrorResponse
    {
        [JsonPropertyName("error")]
        public StripeError? Error { get; set; }
    }
}
