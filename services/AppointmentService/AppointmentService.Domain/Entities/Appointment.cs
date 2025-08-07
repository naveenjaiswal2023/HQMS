using AppointmentService.Domain.Common;
using AppointmentService.Domain.Enums;
using System;

namespace AppointmentService.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        public Guid Id { get; set; }

        // Required References
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid HospitalId { get; set; }

        // Scheduling Info
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }

        public AppointmentStatus Status { get; private set; } = AppointmentStatus.Scheduled;

        // Optional Details
        public string? Remarks { get; set; }
        public AppointmentType Type { get; set; } = AppointmentType.Scheduled;
        public bool IsQueueGenerated { get; set; }
        public Guid? CreatedByUserId { get; set; } // AuthService UserId (Admin/POD)

        public DateTime BookedAt { get; set; } = DateTime.UtcNow;

        // Constructor for EF Core
        private Appointment() { }

        // Public constructor
        public Appointment(Guid id, Guid doctorId, Guid patientId, Guid departmentId, Guid hospitalId, DateTime scheduledDate, TimeSpan scheduledTime, AppointmentType type, Guid? createdByUserId = null, string? remarks = null)
        {
            Id = id;
            DoctorId = doctorId;
            PatientId = patientId;
            DepartmentId = departmentId;
            HospitalId = hospitalId;
            ScheduledDate = scheduledDate;
            ScheduledTime = scheduledTime;
            Type = type;
            CreatedByUserId = createdByUserId;
            Remarks = remarks;
            BookedAt = DateTime.UtcNow;

            // Optionally: Raise domain event here
            //AddDomainEvent(new AppointmentCreatedEvent(Id));
        }

        public void Cancel()
        {
            Status = AppointmentStatus.Cancelled;
            // AddDomainEvent(new AppointmentCancelledEvent(Id));
        }

        public void MarkAsCompleted()
        {
            Status = AppointmentStatus.Completed;
        }

        public void Reschedule(DateTime newDate, TimeSpan newTime)
        {
            ScheduledDate = newDate;
            ScheduledTime = newTime;
            Status = AppointmentStatus.Rescheduled;
        }
    }
}
