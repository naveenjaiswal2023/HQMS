using Contracts.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Domain.Interfaces.ExternalServices
{
    public interface IAppointmentServiceClient
    {
        Task<AppointmentInfoDto> GetAppointmentInfoAsync(Guid appointmentId);
        Task<List<AppointmentInfoDto>> GetUpcomingAppointmentsAsync(int minutesAhead);
    }
}
