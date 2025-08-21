using QueueService.Domain.Common;
using QueueService.Domain.Interfaces;
using System;

namespace QueueService.Domain.Events
{
    public class QueueItemCalledEvent : BaseDomainEvent, IDomainEvent
    {
        public QueueItemCalledEvent()
        {
            
        }
        public Guid QueueItemId { get; init; }
        public string QueueNumber { get; init; }
        public string PatientName { get; init; }
        public string DoctorName { get; init; }
        //public string RoomNumber { get; init; }
        public Guid DepartmentId { get; init; }
        public Guid HospitalId { get; init; }
        public Guid DoctorId { get; init; }

        public DateTime CalledAt { get; init; }

        public QueueItemCalledEvent(
            Guid queueItemId,
            string queueNumber,
            string patientName,
            string doctorName,
            //string roomNumber,
            Guid departmentId,
            Guid hospitalId,
            Guid doctorid,
            DateTime calledAt)
        {
            QueueItemId = queueItemId;
            QueueNumber = queueNumber;
            PatientName = patientName;
            DoctorName = doctorName;
            //RoomNumber = roomNumber;
            DepartmentId = departmentId;
            HospitalId = hospitalId;
            DoctorId = doctorid;
            CalledAt = calledAt;
        }
    }
}