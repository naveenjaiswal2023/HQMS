using MediatR;
using PatientService.Application.Commands;
using PatientService.Application.Common.Models; // For Result<T>
using PatientService.Application.Exceptions;
using PatientService.Domain.Interfaces;

namespace PatientService.Application.Handlers.Commands
{
    public class CompletePatientRegistrationCommandHandler
        : IRequestHandler<CompletePatientRegistrationCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompletePatientRegistrationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(CompletePatientRegistrationCommand request, CancellationToken cancellationToken)
        {
            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(request.PatientId);

            if (patient == null)
            {
                return Result<bool>.Failure($"Patient with ID {request.PatientId} not found.");
            }

            // Update domain state
            patient.ActivateRegistration();

            // If repository requires explicit update
            await _unitOfWork.PatientRepository.UpdateAsync(patient);

            // Commit changes
            var saveResult = await _unitOfWork.SaveAsync(cancellationToken);

            if (saveResult > 0)
            {
                return Result<bool>.Success(true);
            }

            return Result<bool>.Failure("Failed to complete patient registration. Please try again.");
        }
    }
}
