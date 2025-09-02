using PatientService.Domain.Common;
using PatientService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Domain.Events
{
    public class PatientPaymentFailedEvent : BaseDomainEvent, IDomainEvent
    {
        public Guid PatientId { get; }
        public string UHID { get; }
        public string Reason { get; }

        public PatientPaymentFailedEvent(Guid patientId, string uhid, string reason)
        {
            PatientId = patientId;
            UHID = uhid;
            Reason = reason;
        }
    }
}
