using MediatR;
using QueueService.Domain.Common;
using QueueService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Domain.Events
{
    public class PatientQueuedEvent : BaseDomainEvent, IDomainEvent
    {
        public Guid QueueItemId { get; }
        public string QueueNumber { get; }
        public Guid DoctorId { get; }
        public Guid PatientId { get; }
        public Guid AppointmentId { get; }
        public DateTime JoinedAt { get; }

        public PatientQueuedEvent(Guid queueItemId, string queueNumber, Guid doctorId, Guid patientId, Guid appointmentId, DateTime joinedAt)
        {
            QueueItemId = queueItemId;
            QueueNumber = queueNumber;
            DoctorId = doctorId;
            PatientId = patientId;
            AppointmentId = appointmentId;
            JoinedAt = joinedAt;
        }
    }
}
