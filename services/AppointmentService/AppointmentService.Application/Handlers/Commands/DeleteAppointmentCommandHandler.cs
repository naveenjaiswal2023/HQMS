using System;
using System.Threading;
using System.Threading.Tasks;
using AppointmentService.Application.Commands;
using AppointmentService.Domain.Interfaces;
using MediatR;

namespace AppointmentService.Application.Handlers.Commands
{
    public class DeleteAppointmentCommandHandler : IRequestHandler<DeleteAppointmentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentRepository _appointmentRepository;

        public DeleteAppointmentCommandHandler(
            IUnitOfWork unitOfWork,
            IAppointmentRepository appointmentRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
        }

        public async Task<bool> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.Id);

            if (appointment == null)
            {
                // Could throw a NotFoundException instead if you want global handling
                return false;
            }

            _appointmentRepository.DeleteAsync(appointment,cancellationToken);

            await _unitOfWork.SaveAsync(cancellationToken);

            return true;
        }
    }
}
