using MediatR;
using QueueService.Application.DTOs;
using QueueService.Application.Queries;
using SharedInfrastructure.ExternalServices.Interfaces;
using System.Collections.Generic;
using System.Linq;
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

            var result = externalAppointments.Select(a => new AppointmentDto
            {
                PatientId = a.PatientId,
                AppointmentDateTime = a.AppointmentDate,
                DoctorId = a.DoctorId,
            }).ToList();

            return result;
        }

    }
}
