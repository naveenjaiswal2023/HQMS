using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Domain.Models.Payments
{
    public record PaymentInitiationResponse(
        bool IsSuccess,
        string PaymentUrl,
        string GatewayTransactionId,
        string ErrorMessage
    );
}
