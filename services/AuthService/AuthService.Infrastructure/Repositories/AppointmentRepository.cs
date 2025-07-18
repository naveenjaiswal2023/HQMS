using HQMS.QueueService.Domain.Entities;
using HQMS.QueueService.Domain.Interfaces;
using HQMS.QueueService.Infrastructure.Persistence;
using HQMS.QueueService.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HQMS.QueueService.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AuthDbContext _context;

        public AppointmentRepository(AuthDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Appointment entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Appointment entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Appointment>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Appointment>> GetAppointmentsWithinNextMinutesAsync(int minutes)
        {
            var now = DateTime.UtcNow;
            var threshold = now.AddMinutes(minutes);
            var today = now.Date;

            return await _context.Appointments
                .Where(a => a.AppointmentTime.Date == today) // Ensure it's today only
                .Where(a => a.AppointmentTime <= threshold && !a.QueueGenerated)
                .ToListAsync();
       }

        public async Task<Appointment?> GetByDoctorAndPatientAsync(Guid doctorId, Guid patientId)
        {
            var today = DateTime.UtcNow.Date;

            return await _context.Appointments
                .FirstOrDefaultAsync(a =>
                    a.DoctorId == doctorId &&
                    a.PatientId == patientId &&
                    a.AppointmentTime.Date == today);
        }


        public Task<Appointment> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Appointment entity)
        {
            throw new NotImplementedException();
        }
    }
}
