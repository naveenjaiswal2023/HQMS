using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Enums;
using AppointmentService.Domain.Interfaces;
using AppointmentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AppointmentService.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppointmentDbContext _dbContext;

        public AppointmentRepository(AppointmentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Appointment entity)
        {
            await _dbContext.Appointments.AddAsync(entity);
        }

        public async Task UpdateAsync(Appointment entity)
        {
            _dbContext.Appointments.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Appointment entity)
        {
            _dbContext.Appointments.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<Appointment> GetByIdAsync(Guid id)
        {
            return await _dbContext.Appointments.FindAsync(id);
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _dbContext.Appointments
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetUpcomingAppointmentsAsync(DateTime fromTime, DateTime toTime)
        {
            return await _dbContext.Appointments
                .AsNoTracking()
                .Where(a =>
                    a.ScheduledDate >= fromTime &&
                    a.ScheduledDate <= toTime &&
                    a.IsQueueGenerated == false &&
                    a.Status == AppointmentStatus.Scheduled)
                .ToListAsync();
        }
    }
}