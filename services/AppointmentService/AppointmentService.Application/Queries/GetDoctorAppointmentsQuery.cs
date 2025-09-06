using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Queries
{
    public class GetDoctorAppointmentsQuery
    {
        public GetDoctorAppointmentsQuery(Guid doctorId, bool? upcomingOnly = null)
        {
            DoctorId = doctorId;
            UpcomingOnly = upcomingOnly;
        }

        public Guid DoctorId { get; }
        public bool? UpcomingOnly { get; }
    }

}
