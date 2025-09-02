using MediatR;
using PatientService.Application.Common.Models;
using PatientService.Application.DTOs;
using PatientService.Application.Queries;
using PatientService.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Application.Handlers.Queries
{
    public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, Result<PatientDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPatientByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PatientDto>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(request.PatientId);

            if (patient == null)
            {
                return Result<PatientDto>.Failure($"Patient with ID {request.PatientId} not found");
            }

            var dto = new PatientDto
            {
                Id = patient.Id,
                UHID = patient.UHID,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Gender = patient.Gender,
                DateOfBirth = patient.DateOfBirth,
                PhoneNumber = patient.PhoneNumber,
                Email = patient.Email,
                Address = patient.Address,
                EmergencyContact = patient.EmergencyContact,
                MedicalHistory = patient.MedicalHistory,
                CreatedAt = patient.CreatedAt
            };

            return Result<PatientDto>.Success(dto);
        }
    }
}
