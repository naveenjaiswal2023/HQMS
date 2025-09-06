using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Interfaces;
using AppointmentService.Domain.ValueObjects;
using AppointmentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AppointmentService.Infrastructure.Repositories
{
    public class DoctorScheduleRepository : Repository<DoctorSchedule>, IDoctorScheduleRepository
    {
        private readonly AppointmentDbContext _context;

        public DoctorScheduleRepository(AppointmentDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DoctorSchedule>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default)
        {
            return await _context.DoctorSchedules
                .Where(s => s.DoctorId == doctorId)
                .ToListAsync(cancellationToken);
        }

        public async Task<DoctorSchedule?> GetByDoctorAndDayAsync(Guid doctorId, DayOfWeek dayOfWeek, CancellationToken cancellationToken = default)
        {
            return await _context.DoctorSchedules
                .FirstOrDefaultAsync(s => s.DoctorId == doctorId && s.DayOfWeek == dayOfWeek, cancellationToken);
        }

        public async Task<IEnumerable<TimeSlot>> GetAvailableSlotsAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken = default)
        {
            var schedule = await _context.DoctorSchedules
                .FirstOrDefaultAsync(s => s.DoctorId == doctorId
                                       && s.DayOfWeek == date.DayOfWeek
                                       && s.IsActive,
                                     cancellationToken);

            if (schedule == null)
                return Enumerable.Empty<TimeSlot>();

            return schedule.GetAvailableTimeSlots();
        }

    }
}
