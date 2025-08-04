using AppointmentService.Application.DTOs;
using AppointmentService.Application.Queries;
using AppointmentService.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService.Application.Handlers.Queries
{
    public class GetUpcomingAppointmentsQueryHandler : IRequestHandler<GetUpcomingAppointmentsQuery, List<AppointmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUpcomingAppointmentsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AppointmentDto>> Handle(GetUpcomingAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var appointments = await _unitOfWork.Appointments
                .GetUpcomingAppointmentsAsync(request.FromTime, request.ToTime);

            return appointments.Select(a => new AppointmentDto
            {
                AppointmentId = a.Id,
                PatientId = a.PatientId,
                DoctorId = a.DoctorId,
                DepartmentId = a.DepartmentId,
                HospitalId = a.HospitalId,
                ScheduledDate = a.ScheduledDate,
                ScheduledTime = a.ScheduledTime,
                Status = a.Status
            }).ToList();
        }
    }
}
