using MediatR;
using PatientService.Application.Common.Models;
using PatientService.Application.DTOs;
using PatientService.Application.Queries;
using PatientService.Domain.Interfaces;

namespace PatientService.Application.Handlers.Queries
{
    public class GetAllPatientsQueryHandler
        : IRequestHandler<GetAllPatientsQuery, Result<IEnumerable<PatientDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllPatientsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<PatientDto>>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
        {
            var patients = await _unitOfWork.PatientRepository.GetAllAsync();

            if (patients == null || !patients.Any())
                return Result<IEnumerable<PatientDto>>.Failure("No patients found.");

            var dtoList = patients.Select(p => new PatientDto
            {
                Id = p.Id,
                UHID=p.UHID,
                FirstName = p.FirstName,
                LastName = p.LastName,
                PhoneNumber = p.PhoneNumber,
                Gender = p.Gender,
                DateOfBirth = p.DateOfBirth,
                Email=p.Email,
                Address=p.Address,
                EmergencyContact=p.EmergencyContact,
                
                MedicalHistory=p.MedicalHistory,
                CreatedAt=p.CreatedAt,
            });

            return Result<IEnumerable<PatientDto>>.Success(dtoList);
        }
    }
}
