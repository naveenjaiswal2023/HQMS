using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Domain.Interfaces
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<List<Appointment>> GetUpcomingAppointmentsAsync(DateTime fromTime, DateTime toTime);

    }
}
