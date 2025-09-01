using MediatR;
using PatientService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Queries
{
    public record GetPatientByIdQuery(Guid PatientId) : IRequest<PatientDto>;
}
