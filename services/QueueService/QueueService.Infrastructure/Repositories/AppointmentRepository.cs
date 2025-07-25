//using QueueService.Application.DTOs;
//using QueueService.Application.Interfaces;
//using QueueService.Infrastructure.Persistence;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace QueueService.Infrastructure.Repositories
//{
//    public class AppointmentRepository : IAppointmentRepository
//    {
//        private readonly QueueDbContext _dbContext;

//        public AppointmentRepository(QueueDbContext dbContext)
//        {
//            _dbContext = dbContext;
//        }

//        public async Task<List<AppointmentDto>> GetUpcomingAppointmentsAsync(DateTime fromTime, DateTime toTime)
//        {
//            return await _dbContext.Appointments
//                .Where(a => a.AppointmentDate >= fromTime && a.AppointmentDate <= toTime)
//                .Select(a => new AppointmentDto
//                {
//                    Id = a.Id,
//                    PatientId = a.PatientId,
//                    DoctorId = a.DoctorId,
//                    AppointmentDate = a.AppointmentDate,
//                    Status = a.Status
//                })
//                .ToListAsync();
//        }
//    }
//}
