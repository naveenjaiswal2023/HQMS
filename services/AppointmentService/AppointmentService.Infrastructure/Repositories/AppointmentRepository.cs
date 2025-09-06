using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Enums;
using AppointmentService.Domain.Interfaces;
using AppointmentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AppointmentService.Infrastructure.Repositories
{
    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        private readonly AppointmentDbContext _dbContext;

        public AppointmentRepository(AppointmentDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Appointments
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<List<Appointment>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Appointments
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Appointment>> GetUpcomingAppointmentsAsync(DateTime fromTime, DateTime toTime, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Appointments
                .AsNoTracking()
                .Where(a =>
                    a.ScheduledDate >= fromTime &&
                    a.ScheduledDate <= toTime &&
                    !a.IsQueueGenerated &&
                    a.Status == AppointmentStatus.Scheduled)
                .OrderBy(a => a.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Appointments
                .Where(a => a.PatientId == patientId)
                .OrderBy(a => a.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Appointment>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Appointments
                .Where(a => a.DoctorId == doctorId)
                .OrderBy(a => a.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Appointments
                .Where(a => a.ScheduledDate >= startDate && a.EndDateTime <= endDate)
                .OrderBy(a => a.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasConflictingAppointmentAsync(
            Guid doctorId,
            DateTime startDateTime,
            DateTime endDateTime,
            Guid? excludeAppointmentId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Appointments
                .Where(a => a.DoctorId == doctorId && a.Status == AppointmentStatus.Scheduled)
                .Where(a => a.ScheduledDate < endDateTime && a.EndDateTime > startDateTime);

            if (excludeAppointmentId.HasValue)
            {
                query = query.Where(a => a.Id != excludeAppointmentId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<List<Appointment>> GetByHospitalIdAsync(Guid hospitalId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Appointments
                .Where(a => a.HospitalId == hospitalId)
                .OrderBy(a => a.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Appointment>> GetCurrentAppointmentsAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            return await _dbContext.Appointments
                .Where(a => a.Status == AppointmentStatus.InProgress ||
                            (a.Status == AppointmentStatus.Scheduled && a.ScheduledDate <= now && a.EndDateTime >= now))
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Appointment>> GetTodayAppointmentsAsync(CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.Date;
            return await _dbContext.Appointments
                .Where(a => a.ScheduledDate.Date == today)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Appointment>> GetUpcomingAppointmentsByPatientAsync(Guid patientId, int days = 30, CancellationToken cancellationToken = default)
        {
            var toDate = DateTime.UtcNow.AddDays(days);
            return await _dbContext.Appointments
                .Where(a => a.PatientId == patientId &&
                            a.ScheduledDate >= DateTime.UtcNow &&
                            a.ScheduledDate <= toDate)
                .OrderBy(a => a.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Appointment>> GetUpcomingAppointmentsByDoctorAsync(Guid doctorId, int days = 7, CancellationToken cancellationToken = default)
        {
            var toDate = DateTime.UtcNow.AddDays(days);
            return await _dbContext.Appointments
                .Where(a => a.DoctorId == doctorId &&
                            a.ScheduledDate >= DateTime.UtcNow &&
                            a.ScheduledDate <= toDate)
                .OrderBy(a => a.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Appointment>> GetAppointmentsDueForReminderAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            return await _dbContext.Appointments
                .Where(a => a.SendReminder &&
                            !a.ReminderSentAt.HasValue &&
                            a.Status == AppointmentStatus.Scheduled &&
                            a.ScheduledDate.AddMinutes(-a.ReminderMinutesBefore) <= now)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetTotalAppointmentsCountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Appointments.CountAsync(cancellationToken);
        }

        public async Task<int> GetAppointmentsCountByStatusAsync(AppointmentStatus status, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Appointments.CountAsync(a => a.Status == status, cancellationToken);
        }

        public async Task<List<Appointment>> SearchAppointmentsAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Appointment>();

            return await _dbContext.Appointments
                .Where(a => a.Title.Contains(searchTerm) ||
                            a.Description.Contains(searchTerm) ||
                            a.Location.Contains(searchTerm) ||
                            (a.Notes != null && a.Notes.Contains(searchTerm)))
                .OrderBy(a => a.ScheduledDate)
                .ToListAsync(cancellationToken);
        }
    }
}
