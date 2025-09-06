using System;

namespace AppointmentService.Application.Commands
{
    public class UpdateAppointmentCommand
    {
        public Guid Id { get; }
        public string Title { get; }
        public string Description { get; }

        public DateTime StartDateTime { get; }
        public DateTime EndDateTime { get; }
        public string Location { get; }
        public string Notes { get; set; }

        public UpdateAppointmentCommand(
            Guid id,
            string title,
            string description,
            DateTime startDateTime,
            DateTime endDateTime,
            string location,
            string notes = ""
            )
        {
            Id = id;
            Title = title ?? throw new ArgumentNullException(nameof(title));

            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }
    }
}
