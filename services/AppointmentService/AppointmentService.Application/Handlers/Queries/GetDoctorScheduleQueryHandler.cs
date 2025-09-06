using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppointmentService.Application.Queries;
using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Interfaces;
using MediatR;

namespace AppointmentService.Application.Handlers.Queries
{
    public class GetDoctorScheduleQueryHandler : IRequestHandler<GetDoctorScheduleQuery, IEnumerable<DoctorSchedule>>
    {
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;

        public GetDoctorScheduleQueryHandler(IDoctorScheduleRepository doctorScheduleRepository)
        {
            _doctorScheduleRepository = doctorScheduleRepository ?? throw new ArgumentNullException(nameof(doctorScheduleRepository));
        }

        public async Task<IEnumerable<DoctorSchedule>> Handle(GetDoctorScheduleQuery request, CancellationToken cancellationToken)
        {
            var schedules = await _doctorScheduleRepository.GetByDoctorIdAsync(request.DoctorId, cancellationToken);

            return schedules ?? Enumerable.Empty<DoctorSchedule>();
        }
    }
}
