
using Contracts.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Domain.Interfaces.ExternalServices
{
    public interface IPatientServiceClient
    {
        Task<PatientInfoDto> GetPatientInfoAsync(Guid patientId);
    }
}
