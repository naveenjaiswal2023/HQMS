using PaymentService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Domain.Models.Payments
{
    public record PaymentInitiationRequest(
        string TransactionId,
        decimal Amount,
        string Currency,
        PaymentMethod PaymentMethod,
        string CustomerEmail,
        string CustomerPhone,
        string Description
    );
}
