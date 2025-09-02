using MediatR;
using PatientService.Application.Common.Models; // Assuming Result<T> lives here

namespace PatientService.Application.Commands
{
    public class CompletePatientRegistrationCommand : IRequest<Result<bool>>
    {
        public Guid PatientId { get; set; }

        // Parameterless constructor (needed for model binding / serialization)
        public CompletePatientRegistrationCommand() { }

        // Full constructor
        public CompletePatientRegistrationCommand(Guid patientId)
        {
            PatientId = patientId;
        }
    }
}
