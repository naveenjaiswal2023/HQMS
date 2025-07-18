using HQMS.QueueService.Application.DTOs;
using HQMS.QueueService.Domain.Entities;
using HQMS.QueueService.Domain.Enums;
using HQMS.QueueService.Domain.Interfaces;
using HQMS.QueueService.Infrastructure.Persistence;
using HQMS.QueueService.Shared.Helpers;
using HQMS.QueueService.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HQMS.QueueService.Infrastructure.Repositories
{
    public class QueueItemRepository : IQueueItemRepository
    {
        private readonly AuthDbContext _context;

        public QueueItemRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(QueueItem item)
        {
            await _context.QueueItems.AddAsync(item);
        }

        public async Task DeleteAsync(QueueItem entity)
        {
            _context.QueueItems.Remove(entity);
            await Task.CompletedTask; // if your UoW handles SaveChangesAsync
        }

        public async Task<IEnumerable<QueueItem>> GetAllAsync()
        {
            return await _context.QueueItems.ToListAsync();
        }

        public async Task<QueueItem> GetByIdAsync(Guid id)
        {
            return await _context.QueueItems.FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<int> GetNextPositionAsync(Guid doctorId)
        {
            var maxPosition = await _context.QueueItems
                .Where(q => q.DoctorId == doctorId && q.Status != QueueStatus.Completed)
                .MaxAsync(q => (int?)q.Position) ?? 0;

            return maxPosition + 1;
        }

        public async Task<List<QueueItem>> GetWaitingPatientsByDepartmentAsync(Guid departmentId)
        {
            return await _context.QueueItems
                .Where(q => q.DepartmentId == departmentId && q.Status == 0)
                .ToListAsync();
        }
        public async Task<List<QueueItem>> GetQueueItemsForDoctorByDateAsync(Guid doctorId, DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _context.QueueItems
                .Where(q => q.DoctorId == doctorId && q.JoinedAt >= startOfDay && q.JoinedAt < endOfDay)
                .OrderBy(q => q.Position)
                .ToListAsync();
        }
        public async Task<QueueItem?> GetByAppointmentIdAsync(Guid appointmentId)
        {
            return await _context.QueueItems
                .FirstOrDefaultAsync(q => q.AppointmentId == appointmentId);
        }
        public async Task UpdateAsync(QueueItem entity)
        {
            _context.QueueItems.Update(entity);
            await Task.CompletedTask;
        }

        public async Task<List<QueueDashboardItemDto>> GetDashboardDataAsync()
        {
            var today = DateTime.Today;

            var result = await (
                from queue in _context.QueueItems
                join appointment in _context.Appointments
                    on queue.AppointmentId equals appointment.Id
                where !queue.IsDeleted &&
                      appointment.AppointmentTime.Date == today
                orderby queue.QueueNumber
                select new QueueDashboardItemDto
                {
                    QueueNumber = queue.QueueNumber,
                    PatientName = "Unknown",
                    DoctorName = "Unknown",
                    AppointmentTime = appointment.AppointmentTime,
                    Department = queue.Department,
                    Status = EnumHelper.GetName<QueueStatus>(queue.Status)
                }
            ).ToListAsync();

            return result;
        }


    }
}
