using MediatR;
using QueueService.Application.DTOs;
using QueueService.Application.Queries;
using QueueService.Domain.Interfaces.ExternalServices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService.Application.Handlers.Queries
{
    public class GetUpcomingAppointmentsQueryHandler : IRequestHandler<GetUpcomingAppointmentsQuery, List<AppointmentDto>>
    {
        private readonly IAppointmentServiceClient _appointmentClient;

        public GetUpcomingAppointmentsQueryHandler(IAppointmentServiceClient appointmentClient)
        {
            _appointmentClient = appointmentClient;
        }

        public async Task<List<AppointmentDto>> Handle(GetUpcomingAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var externalAppointments = await _appointmentClient.GetUpcomingAppointmentsAsync(request.MinutesAhead);

            var result = externalAppointments.Select(a => new AppointmentDto
            {
                AppointmentId = a.Id,
                PatientId = a.PatientId,
                AppointmentDateTime = a.AppointmentDate,
                DoctorId = a.DoctorId,
                DepartmentId = a.DepartmentId
               
                // Map any other fields as necessary
            }).ToList();

            return result;
        }

    }
}
