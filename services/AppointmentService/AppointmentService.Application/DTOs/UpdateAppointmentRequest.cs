using System;

namespace AppointmentService.Application.DTOs
{
    public class UpdateAppointmentRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Notes { get; set; }
    }
}
