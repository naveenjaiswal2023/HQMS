using MediatR;

namespace HQMS.QueueService.Application.Commands
{
    public class GenerateDoctorQueueCommand : IRequest<bool>
    {
        public Guid DoctorId { get; private set; }
        public Guid PatientId { get; private set; }
        public Guid AppointmentId { get; private set; }
        public string PatientName { get; private set; }
        public string DepartmentName { get; private set; }
        public Guid DepartmentId { get; private set; }
        public DateTime AppointmentTime { get; private set; }

        public GenerateDoctorQueueCommand(
            Guid doctorId,
            Guid patientId,
            Guid appointmentId,
            string patientName,
            string departmentName,
            Guid departmentId,
            DateTime appointmentTime)
        {
            DoctorId = doctorId;
            PatientId = patientId;
            AppointmentId = appointmentId;
            PatientName = patientName;
            DepartmentName = departmentName;
            DepartmentId = departmentId;
            AppointmentTime = appointmentTime;
        }
    }

}
