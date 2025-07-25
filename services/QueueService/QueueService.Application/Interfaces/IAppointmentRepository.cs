using QueueService.Application.DTOs;
using QueueService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.Interfaces
{
    public interface IAppointmentRepository : IRepository<AppointmentDto>
    {
        Task<List<AppointmentDto>> GetUpcomingAppointmentsAsync(DateTime fromTime, DateTime toTime);
    }
}
