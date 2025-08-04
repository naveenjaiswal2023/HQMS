using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Domain.Enums
{
    public enum AppointmentStatus
    {
        Scheduled = 0,
        Cancelled = 1,
        Completed = 2,
        NoShow = 3,
        Rescheduled = 4
    }
}
