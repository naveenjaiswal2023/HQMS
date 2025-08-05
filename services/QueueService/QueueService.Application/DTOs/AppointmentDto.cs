using AppointmentService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.DTOs
{
    public class AppointmentDto
    {
        public Guid AppointmentId { get; set; }

        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid HospitalId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public AppointmentStatus Status { get; set; } = new AppointmentStatus();
        public DateTime AppointmentDateTime { get; set; } // Combined date + time for easier processing
    }
}
