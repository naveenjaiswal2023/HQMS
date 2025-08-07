using AppointmentService.Application.Commands;
using AppointmentService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Handlers.Commands
{
    public class UpdateAppointmentStatusCommandHandler : IRequestHandler<UpdateAppointmentStatusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateAppointmentStatusCommandHandler> _logger;

        public UpdateAppointmentStatusCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateAppointmentStatusCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateAppointmentStatusCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.AppointmentId);

            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {request.AppointmentId} not found.");

            appointment.IsQueueGenerated = request.Status;
            _unitOfWork.Appointments.UpdateAsync(appointment);

            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Updated appointment {AppointmentId} status to {Status}", request.AppointmentId, request.Status);
            return true;
        }
    }

}
