using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Appointments
{
    public class AppointmentInfoDto
    {
        public Guid Id { get; set; }                     // Unique appointment ID
        public Guid PatientId { get; set; }              // Reference to Patient
        public Guid DoctorId { get; set; }               // Reference to Doctor
        public Guid DepartmentId { get; set; }           // Reference to Department
        public Guid HospitalId { get; set; }             // Reference to Hospital

        public DateTime AppointmentDate { get; set; }    // Date and time of the appointment
        public string? Slot { get; set; }                 // Time slot info (e.g., "09:00 AM - 09:30 AM")
        public string? Status { get; set; }               // e.g., "Scheduled", "Completed", "Cancelled"
        public string? Remarks { get; set; }              // Optional remarks

        // Optional additional metadata
        public bool IsTeleConsultation { get; set; }
    }
}
