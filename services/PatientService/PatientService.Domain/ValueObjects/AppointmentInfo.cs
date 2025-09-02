using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Domain.ValueObjects
{
    public class AppointmentInfo
    {
        public Guid Id { get; }
        public Guid PatientId { get; }
        public Guid DoctorId { get; }
        public Guid DepartmentId { get; }
        public DateTime AppointmentDate { get; }

        public AppointmentInfo(Guid id, Guid patientId, Guid doctorId, Guid departmentId, DateTime appointmentDate)
        {
            Id = id;
            PatientId = patientId;
            DoctorId = doctorId;
            DepartmentId = departmentId;
            AppointmentDate = appointmentDate;
        }
    }
}
