using System;
using MediatR;
using AppointmentService.Domain.Enums;

namespace AppointmentService.Application.Commands
{
    public class CreateAppointmentCommand : IRequest<CreateAppointmentResponse>
    {
        public string Title { get; }
        public string Description { get; }
        public DateTime StartDateTime { get; }
        public DateTime EndDateTime { get; }
        public Guid PatientId { get; }
        public Guid DoctorId { get; }
        public Guid HospitalId { get; }
        public string Location { get; }
        public AppointmentType Type { get; }
        public decimal Fee { get; }
        public bool SendReminder { get; }
        public int ReminderMinutesBefore { get; }

        public CreateAppointmentCommand(
            string title,
            string description,
            DateTime startDateTime,
            DateTime endDateTime,
            Guid patientId,
            Guid doctorId,
            Guid hospitalId,
            string location,
            AppointmentType type,
            decimal fee,
            bool sendReminder = true,
            int reminderMinutesBefore = 30)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            PatientId = patientId;
            DoctorId = doctorId;
            HospitalId = hospitalId;
            Location = location ?? throw new ArgumentNullException(nameof(location));
            Type = type;
            Fee = fee;
            SendReminder = sendReminder;
            ReminderMinutesBefore = reminderMinutesBefore;
        }
    }
}
