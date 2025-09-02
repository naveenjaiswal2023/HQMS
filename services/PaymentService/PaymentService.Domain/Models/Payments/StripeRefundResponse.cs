using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PaymentService.Domain.Models.Payments
{
    public class StripeRefundResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("payment_intent")]
        public string PaymentIntent { get; set; } = string.Empty;

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }
}
