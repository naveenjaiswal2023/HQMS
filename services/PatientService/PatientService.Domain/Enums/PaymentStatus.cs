using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending,
        Processing,
        Success,
        Failed,
        Refunded,
        PartiallyRefunded
    }
}
