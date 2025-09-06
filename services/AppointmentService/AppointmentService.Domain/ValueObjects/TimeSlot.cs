using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Domain.ValueObjects
{
    public record TimeSlot(TimeSpan StartTime, TimeSpan EndTime);
}
