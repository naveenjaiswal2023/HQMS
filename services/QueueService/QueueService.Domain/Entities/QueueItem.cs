using QueueService.Domain.Common;
using QueueService.Domain.Enum;
using QueueService.Domain.Events;
using QueueService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Domain.Entities
{
    public class QueueItem : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid DoctorId { get; private set; }
        public Guid PatientId { get; private set; }
        public Guid AppointmentId { get; private set; }
        public Guid DepartmentId { get; private set; }
        public Guid HospitalId { get; private set; }

        public int Position { get; private set; }
        public TimeSpan EstimatedWaitTime { get; private set; }
        public DateTime JoinedAt { get; private set; }
        public DateTime? CalledAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public DateTime? SkippedAt { get; private set; }
        public DateTime? CancelledAt { get; private set; }


        public QueueStatus Status { get; private set; }
        public string QueueNumber { get; private set; }

        public PatientInfo PatientInfo { get; private set; }
        public DoctorInfo DoctorInfo { get; private set; }

        private QueueItem() { }

        public QueueItem(
            Guid id,
            Guid doctorId,
            Guid patientId,
            Guid appointmentId,
            Guid departmentId,
            Guid hospitalId,
            int position,
            TimeSpan estimatedWaitTime,
            string queueNumber,
            PatientInfo patientInfo,
            DoctorInfo doctorInfo)
        {
            if (position < 1)
                throw new ArgumentException("Position must be at least 1");

            Id = id;
            DoctorId = doctorId;
            PatientId = patientId;
            AppointmentId = appointmentId;
            DepartmentId = departmentId;
            HospitalId = hospitalId;
            Position = position;
            EstimatedWaitTime = estimatedWaitTime;
            QueueNumber = queueNumber ?? throw new ArgumentNullException(nameof(queueNumber));
            JoinedAt = DateTime.UtcNow;
            Status = QueueStatus.Pending;

            PatientInfo = patientInfo;
            DoctorInfo = doctorInfo;

            AddDomainEvent(new PatientQueuedEvent(Id, queueNumber, doctorId, patientId, appointmentId, JoinedAt));
        }
        public void UpdatePosition(int newPosition)
        {
            if (newPosition < 1)
                throw new ArgumentException("Position must be at least 1");
            Position = newPosition;
            AddDomainEvent(new QueueItemPositionUpdatedEvent(Id, newPosition));
        }
        public void MarkAsCalled()
        {
            Status = QueueStatus.Called;
            CalledAt = DateTime.UtcNow;

            AddDomainEvent(new QueueItemCalledEvent(
                Id,
                QueueNumber,
                PatientInfo?.Name ?? "Unknown",
                DoctorInfo?.Name ?? "Unknown",
                //DoctorInfo?.RoomNumber ?? "N/A",
                DepartmentId,
                HospitalId,
                DoctorId,
                CalledAt.Value
            ));
        }


        public void MarkAsCompleted()
        {
            if (Status != QueueStatus.Called)
                throw new InvalidOperationException($"Only called items can be marked as completed.");

            Status = QueueStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            AddDomainEvent(new QueueItemCompletedEvent(Id));
        }

        public void MarkAsSkipped()
        {
            if (Status != QueueStatus.Called && Status != QueueStatus.Pending)
                throw new InvalidOperationException($"Cannot skip item in status {Status}.");

            Status = QueueStatus.Skipped;
            SkippedAt = DateTime.UtcNow;
            AddDomainEvent(new QueueItemSkippedEvent(Id));
        }

        public void Cancel()
        {
            if (Status == QueueStatus.Completed || Status == QueueStatus.Cancelled)
                throw new InvalidOperationException($"Cannot cancel item in status {Status}.");

            Status = QueueStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
            AddDomainEvent(new QueueItemCancelledEvent(Id));
        }
    }
}
