using Contracts.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInfrastructure.ExternalServices.Interfaces
{
    public interface IAppointmentServiceClient
    {
        Task<AppointmentInfoDto> GetAppointmentInfoAsync(Guid appointmentId);
        Task<List<AppointmentInfoDto>> GetUpcomingAppointmentsAsync(DateTime fromTime, DateTime toTime);
        Task UpdateAppointmentStatusAsync(Guid appointmentId, bool status);
    }
}
