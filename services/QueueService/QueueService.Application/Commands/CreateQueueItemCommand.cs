using MediatR;
using QueueService.Application.Common;
using QueueService.Application.Common.Models;

namespace QueueService.Application.Commands
{
    public class CreateQueueItemCommand : IRequest<Guid>
    {
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid HospitalId { get; set; }

        public CreateQueueItemCommand(
            Guid doctorId,
            Guid patientId,
            Guid appointmentId,
            Guid departmentId,
            Guid hospitalId
        )
        {
            DoctorId = doctorId;
            PatientId = patientId;
            AppointmentId = appointmentId;
            DepartmentId = departmentId;
            HospitalId = hospitalId;
        }
    }
}
