using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Common.Models
{
    public record PaymentInfo(
        Guid PaymentId,
        string TransactionId,
        string PaymentUrl,
        decimal Amount,
        string Currency
    );
}
