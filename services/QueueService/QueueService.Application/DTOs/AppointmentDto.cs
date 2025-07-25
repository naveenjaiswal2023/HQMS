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

        public Guid HospitalId { get; set; }           // Reference
        public string? HospitalName { get; set; }      // Optional (for display)

        public DateTime AppointmentDateTime { get; set; }
        public string Status { get; set; }             // Scheduled, Completed, Cancelled
        public string QueueNumber { get; set; }
    }
}
