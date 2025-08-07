using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.DTOs
{
    public class UpcomingAppointmentDto
    {
        public Guid AppointmentId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DepartmentId { get; set; }
        public DateTime AppointmentTime { get; set; }
    }
}
