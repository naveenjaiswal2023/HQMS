using MediatR;
using PatientService.Application.Commands;
using PatientService.Application.Exceptions;
using PatientService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Handlers.Commands
{
    public class CompletePatientRegistrationCommandHandler : IRequestHandler<CompletePatientRegistrationCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompletePatientRegistrationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CompletePatientRegistrationCommand request, CancellationToken cancellationToken)
        {
            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(request.PatientId);
            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {request.PatientId} not found");
            }

            // Update domain state
            patient.ActivateRegistration();

            // If UpdateAsync is needed (for detached entity tracking)
            await _unitOfWork.PatientRepository.UpdateAsync(patient);

            // Save changes
            await _unitOfWork.SaveAsync(cancellationToken);

            return true;
        }
    }
}
