using AppointmentService.Application.DTOs;
using MediatR;
using QueueService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppointmentDto = AppointmentService.Application.DTOs.AppointmentDto;

namespace AppointmentService.Application.Queries
{
    public class GetUpcomingAppointmentsQuery : IRequest<List<AppointmentDto>>
    {
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }

        public GetUpcomingAppointmentsQuery(DateTime fromTime, DateTime toTime)
        {
            FromTime = fromTime;
            ToTime = toTime;
        }
    }

}
