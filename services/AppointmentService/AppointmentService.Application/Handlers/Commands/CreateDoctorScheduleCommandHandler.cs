using System;
using System.Threading;
using System.Threading.Tasks;
using AppointmentService.Application.Commands;
using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Interfaces;
using MediatR;

namespace AppointmentService.Application.Handlers.Commands
{
    public class CreateDoctorScheduleCommandHandler : IRequestHandler<CreateDoctorScheduleCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;

        public CreateDoctorScheduleCommandHandler(
            IUnitOfWork unitOfWork,
            IDoctorScheduleRepository doctorScheduleRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _doctorScheduleRepository = doctorScheduleRepository ?? throw new ArgumentNullException(nameof(doctorScheduleRepository));
        }

        public async Task<Guid> Handle(CreateDoctorScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = DoctorSchedule.Create(
                request.DoctorId,
                request.Date,
                request.StartTime,
                request.EndTime,
                request.SlotDurationMinutes
            );

            await _doctorScheduleRepository.AddAsync(schedule, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            return schedule.Id;
        }
    }
}
