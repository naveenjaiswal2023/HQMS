using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Commands
{
    public class CreateDoctorScheduleCommand : IRequest<Guid>
    {
        public CreateDoctorScheduleCommand(Guid doctorId, DayOfWeek date, TimeSpan startTime, TimeSpan endTime, int slotDurationMinutes)
        {
            DoctorId = doctorId;
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            SlotDurationMinutes = slotDurationMinutes;
        }

        public Guid DoctorId { get; }
        public DayOfWeek Date { get; }
        public TimeSpan StartTime { get; }
        public TimeSpan EndTime { get; }
        public int SlotDurationMinutes { get; }
    }
}
