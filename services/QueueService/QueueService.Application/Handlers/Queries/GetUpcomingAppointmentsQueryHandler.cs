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
            var externalAppointments = await _appointmentClient.GetUpcomingAppointmentsAsync(request.FromTime, request.ToTime);

            var result = externalAppointments.Select(a => new AppointmentDto
            {
                AppointmentId = a.AppointmentId,
                PatientId = a.PatientId,
                DoctorId = a.DoctorId,
                DepartmentId = a.DepartmentId,
                HospitalId = a.HospitalId,
                Status = (AppointmentService.Domain.Enums.AppointmentStatus)a.Status,
                AppointmentDateTime = a.AppointmentDateTime // ✅ use existing field
            }).ToList();

            return result;
        }
    }
}
