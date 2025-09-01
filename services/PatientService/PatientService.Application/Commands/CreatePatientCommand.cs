using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Commands
{
    public record CreatePatientCommand(
        string FirstName,
        string LastName,
        DateTime DateOfBirth,
        string PhoneNumber,
        string Email,
        string Address,
        string EmergencyContact,
        string MedicalHistory
    ) : IRequest<CreatePatientResult>;
}
