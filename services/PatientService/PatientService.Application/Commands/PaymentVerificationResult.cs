using PatientService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Commands
{
    public record PaymentVerificationResult(
        bool IsSuccessful,
        PaymentStatus Status,
        string Message
    );
}
