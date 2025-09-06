using AppointmentService.Domain.Common;
using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Interfaces;
using System;

namespace AppointmentService.Domain.Events
{
    public class AppointmentAddedToQueueEvent : BaseDomainEvent,IDomainEvent
    {
        public Appointment Appointment { get; }
        public Guid QueueId { get; } = Guid.NewGuid();

        public AppointmentAddedToQueueEvent(Appointment appointment, Guid queueId)
        {
            Appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
            QueueId = queueId;
        }
    }
}
