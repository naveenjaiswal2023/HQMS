using AppointmentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Queries
{
    public record CurrentAppointmentsResponse(
        int TotalCurrentCount,
        int InProgressCount,
        int TodayScheduledCount,
        IEnumerable<AppointmentSummaryDto> CurrentAppointments,
        IEnumerable<AppointmentSummaryDto> TodayAppointments
    );
}
