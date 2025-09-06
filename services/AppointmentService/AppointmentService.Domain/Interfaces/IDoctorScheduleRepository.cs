using AppointmentService.Domain.Entities;
using AppointmentService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Domain.Interfaces
{
    public interface IDoctorScheduleRepository : IRepository<DoctorSchedule>
    {
        Task<IEnumerable<DoctorSchedule>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default);
        Task<DoctorSchedule?> GetByDoctorAndDayAsync(Guid doctorId, DayOfWeek dayOfWeek, CancellationToken cancellationToken = default);
        Task<IEnumerable<TimeSlot>> GetAvailableSlotsAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken = default);
    }
}
