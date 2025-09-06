using AppointmentService.Domain.Common;
using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Interfaces;
using System;

namespace AppointmentService.Domain.Events
{
    public class AppointmentStartedEvent : BaseDomainEvent,IDomainEvent
    {
        public Appointment Appointment { get; }

        public AppointmentStartedEvent(Appointment appointment)
        {
            Appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
        }
    }
}
