using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.DTOs
{
    public record DoctorScheduleDto(
        Guid Id,
        Guid DoctorId,
        DayOfWeek DayOfWeek,
        TimeSpan StartTime,
        TimeSpan EndTime,
        int SlotDurationMinutes,
        bool IsActive
    );
}
