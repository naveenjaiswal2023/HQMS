using PatientService.Domain.Common;
using PatientService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Domain.Events
{
    public class PatientActivatedEvent : BaseDomainEvent, IDomainEvent
    {
        public Guid PatientId { get; }
        public string UHID { get; }

        public PatientActivatedEvent(Guid patientId, string uhid)
        {
            PatientId = patientId;
            UHID = uhid;
        }
    }
}
