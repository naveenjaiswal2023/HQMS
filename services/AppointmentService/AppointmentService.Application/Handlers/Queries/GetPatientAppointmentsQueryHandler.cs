using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppointmentService.Application.Queries;
using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Interfaces;
using MediatR;

namespace AppointmentService.Application.Handlers.Queries
{
    public class GetPatientAppointmentsQueryHandler : IRequestHandler<GetPatientAppointmentsQuery, IEnumerable<Appointment>>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public GetPatientAppointmentsQueryHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
        }

        public async Task<IEnumerable<Appointment>> Handle(GetPatientAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentRepository.GetByPatientIdAsync(request.PatientId, cancellationToken);

            if (appointments == null)
                return Enumerable.Empty<Appointment>();

            if (request.UpcomingOnly == true)
            {
                var now = DateTime.UtcNow;
                appointments = (List<Appointment>)appointments.Where(a => a.ScheduledDate >= now);
            }

            return appointments;
        }
    }
}
