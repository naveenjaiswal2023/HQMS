using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.Commands
{
    public class CreateQueueItemCommand : IRequest<Guid>
    {
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid HospitalId { get; set; }
        //public string QueueNumber { get; set; }

        public CreateQueueItemCommand(
            Guid doctorId,
            Guid patientId,
            Guid appointmentId,
            Guid departmentId,
            Guid hospitalId
            //string queueNumber
            )
        {
            DoctorId = doctorId;
            PatientId = patientId;
            AppointmentId = appointmentId;
            DepartmentId = departmentId;
            HospitalId = hospitalId;
            //QueueNumber = queueNumber;
        }
    }
}
