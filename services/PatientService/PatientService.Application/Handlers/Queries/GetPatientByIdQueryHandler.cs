using MediatR;
using PatientService.Application.DTOs;
using PatientService.Application.Exceptions;
using PatientService.Application.Queries;
using PatientService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Handlers.Queries
{
    public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, PatientDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPatientByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PatientDto> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(request.PatientId);
            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {request.PatientId} not found");
            }

            var dto = new PatientDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                PhoneNumber = patient.PhoneNumber,
                Email = patient.Email,
                Address = patient.Address,
                EmergencyContact = patient.EmergencyContact,
                MedicalHistory = patient.MedicalHistory,
                CreatedAt = patient.CreatedAt
            };
            return dto;

        }
    }
}
