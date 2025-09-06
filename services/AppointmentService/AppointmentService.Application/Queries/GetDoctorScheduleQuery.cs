using AppointmentService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Queries
{
    public class GetDoctorScheduleQuery : IRequest<IEnumerable<DoctorSchedule>>
    {
        public GetDoctorScheduleQuery(Guid doctorId)
        {
            DoctorId = doctorId;
        }

        public Guid DoctorId { get; }
    }
}
