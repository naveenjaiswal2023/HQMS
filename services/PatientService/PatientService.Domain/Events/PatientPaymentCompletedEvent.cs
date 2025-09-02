using PatientService.Domain.Common;
using PatientService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Domain.Events
{
    public class PatientPaymentCompletedEvent : BaseDomainEvent, IDomainEvent
    {
        public Guid PatientId { get; }
        public string UHID { get; }
        public string PaymentId { get; }

        public PatientPaymentCompletedEvent(Guid patientId, string uhid, string paymentId)
        {
            PatientId = patientId;
            UHID = uhid;
            PaymentId = paymentId;
        }
    }
}
