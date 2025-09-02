using MediatR;
using PatientService.Application.Common.Models;
using PatientService.Application.DTOs;
using System.Collections.Generic;

namespace PatientService.Application.Queries
{
    public class GetAllPatientsQuery : IRequest<Result<IEnumerable<PatientDto>>>
    {
    }
}
