using MediatR;
using PatientService.Application.Common.Models;
using PatientService.Application.DTOs;
using System;

namespace PatientService.Application.Queries
{
    public class GetPatientByIdQuery : IRequest<Result<PatientDto>>
    {
        public Guid PatientId { get; }

        public GetPatientByIdQuery(Guid patientId)
        {
            PatientId = patientId;
        }
    }
}
