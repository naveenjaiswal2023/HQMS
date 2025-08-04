using MediatR;
using QueueService.Application.DTOs;
using System;
using System.Collections.Generic;

namespace QueueService.Application.Queries
{
    /// <summary>
    /// MediatR query to get upcoming appointments between FromTime and ToTime.
    /// </summary>
    public class GetUpcomingAppointmentsQuery : IRequest<List<AppointmentDto>>
    {
        public DateTime FromTime { get; }
        public DateTime ToTime { get; }

        public GetUpcomingAppointmentsQuery(DateTime fromTime, DateTime toTime)
        {
            FromTime = fromTime;
            ToTime = toTime;
        }
    }
}
