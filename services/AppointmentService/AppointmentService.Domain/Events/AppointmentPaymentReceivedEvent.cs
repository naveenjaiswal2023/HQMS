using AppointmentService.Domain.Common;
using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Interfaces;
using System;

namespace AppointmentService.Domain.Events
{
    public class AppointmentPaymentReceivedEvent : BaseDomainEvent,IDomainEvent
    {
        public Appointment Appointment { get; }
        public Guid PaymentId { get; }

        public AppointmentPaymentReceivedEvent(Appointment appointment, Guid paymentId)
        {
            Appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
            PaymentId = paymentId;
        }
    }
}
