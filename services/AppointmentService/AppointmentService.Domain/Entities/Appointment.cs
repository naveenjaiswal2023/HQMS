using AppointmentService.Domain.Common;
using AppointmentService.Domain.Enums;
using AppointmentService.Domain.Events;
using System;

namespace AppointmentService.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime ScheduledDate { get; private set; }
        public TimeSpan ScheduledTime { get; private set; }
        public DateTime EndDateTime { get; private set; }
        public AppointmentStatus Status { get; private set; }
        public AppointmentType Type { get; private set; }
        public string Location { get; private set; }
        public string? Notes { get; private set; }

        // External Service References (not entities)
        public Guid PatientId { get; private set; }
        public Guid DoctorId { get; private set; }
        public Guid HospitalId { get; private set; }
        public Guid DepartmentId { get; private set; }
        public Guid? QueueId { get; private set; } // Link to Queue Service

        // Payment tracking
        public decimal Fee { get; private set; }
        public bool IsPaid { get; private set; }
        public Guid? PaymentId { get; private set; } // Link to Payment Service

        // Reminder settings
        public bool SendReminder { get; private set; }
        public int ReminderMinutesBefore { get; private set; }
        public DateTime? ReminderSentAt { get; private set; }

        // Optional Details
        public string? Remarks { get; set; }
        public bool IsQueueGenerated { get; set; }

        public DateTime BookedAt { get; set; } = DateTime.UtcNow;

        // Constructor for EF Core
        private Appointment() { } // EF Core

        public static Appointment Create(
            string title,
            string description,
            DateTime scheduleDate,
            TimeSpan scheduleTime,
            Guid patientId,
            Guid doctorId,
            Guid hospitalId,
            string location,
            AppointmentType type,
            decimal fee,
            bool sendReminder = true,
            int reminderMinutesBefore = 30)
        {
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                ScheduledDate = scheduleDate,
                ScheduledTime = scheduleTime,
                Status = AppointmentStatus.Scheduled,
                Type = type,
                Location = location,
                PatientId = patientId,
                DoctorId = doctorId,
                HospitalId = hospitalId,
                Fee = fee,
                IsPaid = false,
                SendReminder = sendReminder,
                ReminderMinutesBefore = reminderMinutesBefore
            };

            appointment.AddDomainEvent(new AppointmentCreatedEvent(appointment));
            return appointment;
        }

        public void Update(string title, string description, DateTime scheduleDate,
                          TimeSpan scheduleTime, string location, string? notes = null)
        {
            Title = title;
            Description = description;
            ScheduledDate = scheduleDate;
            ScheduledTime = scheduleTime;
            Location = location;
            Notes = notes;

            AddDomainEvent(new AppointmentUpdatedEvent(this));
        }

        public void Cancel()
        {
            if (Status == AppointmentStatus.Completed)
                throw new DomainException("Cannot cancel a completed appointment");

            Status = AppointmentStatus.Cancelled;
            AddDomainEvent(new AppointmentCancelledEvent(this));
        }

        public void Complete(string? notes = null)
        {
            if (Status != AppointmentStatus.InProgress)
                throw new DomainException("Only in-progress appointments can be completed");

            Status = AppointmentStatus.Completed;
            Notes = notes;
            AddDomainEvent(new AppointmentCompletedEvent(this));
        }

        public void StartAppointment()
        {
            if (Status != AppointmentStatus.Scheduled)
                throw new DomainException("Only scheduled appointments can be started");

            Status = AppointmentStatus.InProgress;
            AddDomainEvent(new AppointmentStartedEvent(this));
        }

        public void MarkAsPaid(Guid paymentId)
        {
            IsPaid = true;
            PaymentId = paymentId;
            AddDomainEvent(new AppointmentPaymentReceivedEvent(this, paymentId));
        }

        public void AssignToQueue(Guid queueId)
        {
            QueueId = queueId;
            AddDomainEvent(new AppointmentAddedToQueueEvent(this, queueId));
        }

        public void MarkReminderSent()
        {
            ReminderSentAt = DateTime.UtcNow;
        }

        public bool ShouldSendReminder()
        {
            if (!SendReminder || ReminderSentAt.HasValue || Status != AppointmentStatus.Scheduled)
                return false;

            var reminderTime = ScheduledDate.AddMinutes(-ReminderMinutesBefore);
            return DateTime.UtcNow >= reminderTime;
        }

        public bool IsCurrentAppointment()
        {
            var now = DateTime.UtcNow;
            return Status == AppointmentStatus.InProgress ||
                   (Status == AppointmentStatus.Scheduled && ScheduledDate <= now && EndDateTime >= now);
        }

        public bool IsTodayAppointment()
        {
            return ScheduledDate.Date == DateTime.UtcNow.Date;
        }

        // Public constructor
        //public Appointment(Guid id, Guid doctorId, Guid patientId, Guid departmentId, Guid hospitalId, DateTime scheduledDate, TimeSpan scheduledTime, AppointmentType type, Guid? createdByUserId = null, string? remarks = null)
        //{
        //    Id = id;
        //    DoctorId = doctorId;
        //    PatientId = patientId;
        //    DepartmentId = departmentId;
        //    HospitalId = hospitalId;
        //    ScheduledDate = scheduledDate;
        //    ScheduledTime = scheduledTime;
        //    Type = type;
        //    Remarks = remarks;
        //    BookedAt = DateTime.UtcNow;

        //    // Optionally: Raise domain event here
        //    //AddDomainEvent(new AppointmentCreatedEvent(Id));
        //}

        public void Reschedule(DateTime newDate, TimeSpan newTime)
        {
            ScheduledDate = newDate;
            ScheduledTime = newTime;
            Status = AppointmentStatus.Rescheduled;
            //AddDomainEvent(new AppointmentRescheduledEvent(this));
        }

    }
}
