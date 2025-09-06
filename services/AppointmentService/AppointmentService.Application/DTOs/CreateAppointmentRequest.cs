using System;
using AppointmentService.Domain.Enums;

namespace AppointmentService.Application.DTOs
{
    public class CreateAppointmentRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid HospitalId { get; set; }
        public string Location { get; set; } = string.Empty;
        public AppointmentType Type { get; set; }
        public decimal Fee { get; set; }
        public bool SendReminder { get; set; } = true;
        public int ReminderMinutesBefore { get; set; } = 30;
    }
}
