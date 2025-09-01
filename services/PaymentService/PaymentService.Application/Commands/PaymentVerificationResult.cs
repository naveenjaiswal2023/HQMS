using PaymentService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Commands
{
    public record PaymentVerificationResult(
        bool IsSuccessful,
        PaymentStatus Status,
        string Message
    );
}
