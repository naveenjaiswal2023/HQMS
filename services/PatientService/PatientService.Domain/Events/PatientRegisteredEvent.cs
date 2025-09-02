using PatientService.Domain.Common;
using PatientService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Domain.Events
{
    public class PatientRegisteredEvent : BaseDomainEvent, IDomainEvent
    {
        public Guid PatientId { get; }
        public string UHID { get; }
        public string FullName { get; }
        public string PhoneNumber { get; }
        public DateTime RegisteredAt { get; }

        public PatientRegisteredEvent(Guid patientId, string uhid, string fullName, string phoneNumber, DateTime registeredAt)
        {
            PatientId = patientId;
            UHID = uhid;
            FullName = fullName;
            PhoneNumber = phoneNumber;
            RegisteredAt = registeredAt;
        }
    }
}
