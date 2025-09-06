using AppointmentService.Domain.Common;
using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Interfaces;
using System;

namespace AppointmentService.Domain.Events
{
    public class AppointmentUpdatedEvent : BaseDomainEvent,IDomainEvent
    {
        public Appointment Appointment { get; }

        public AppointmentUpdatedEvent(Appointment appointment)
        {
            Appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
        }
    }
}
