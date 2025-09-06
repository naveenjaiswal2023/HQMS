using System;
using System.Threading;
using System.Threading.Tasks;
using AppointmentService.Application.Commands;
using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Interfaces;
using MediatR;

namespace AppointmentService.Application.Handlers.Commands
{
    public class CompleteAppointmentCommandHandler : IRequestHandler<CompleteAppointmentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentRepository _appointmentRepository;

        public CompleteAppointmentCommandHandler(
            IUnitOfWork unitOfWork,
            IAppointmentRepository appointmentRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
        }

        public async Task<bool> Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.Id);

            if (appointment == null)
            {
                // You could throw a custom NotFoundException instead of returning false
                return false;
            }

            // Apply domain logic
            appointment.Complete(request.Notes);

            // Update repository
            _appointmentRepository.UpdateAsync(appointment,cancellationToken);

            // Commit transaction
            await _unitOfWork.SaveAsync(cancellationToken);

            return true;
        }
    }
}
