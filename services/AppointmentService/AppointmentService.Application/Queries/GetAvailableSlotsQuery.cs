using AppointmentService.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Queries
{
    public class GetAvailableSlotsQuery : IRequest<List<TimeSlot>>
    {
        public GetAvailableSlotsQuery(Guid doctorId, DateTime date)
        {
            DoctorId = doctorId;
            Date = date;
        }

        public Guid DoctorId { get; }
        public DateTime Date { get; }
    }
}
