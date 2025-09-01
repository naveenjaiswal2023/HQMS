using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Commands
{
    public record CompletePatientRegistrationCommand(Guid PatientId) : IRequest<bool>;
}
