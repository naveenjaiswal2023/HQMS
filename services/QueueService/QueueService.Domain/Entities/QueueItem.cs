using HQMS.API.Domain.Enum;
using QueueService.Domain.Common;
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

            //AddDomainEvent(new PatientQueuedEvent(Id, queueNumber, doctorId, patientId, appointmentId, JoinedAt));
        }

        public void MarkAsCalled()
        {
            Status = QueueStatus.Called;
            CalledAt = DateTime.UtcNow;
            //AddDomainEvent(new QueueItemCalledEvent(Id));
        }

        public void MarkAsCompleted()
        {
            Status = QueueStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }

        public void MarkAsSkipped()
        {
            //Status = QueueStatus.Skipped;
            //AddDomainEvent(new QueueItemSkippedEvent(Id));
        }

        public void Cancel()
        {
            Status = QueueStatus.Cancelled;
            //AddDomainEvent(new QueueItemCancelledEvent(Id));
        }
    }

}
