using Contracts.Patients;
using SharedInfrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInfrastructure.ExternalServices.Interfaces
{
    public interface IPatientServiceClient
    {
        Task<PatientInfoDto> GetPatientInfoAsync(Guid patientId);

        Task<PatientDto?> GetPatientAsync(Guid patientId, CancellationToken cancellationToken = default);
        Task<bool> PatientExistsAsync(Guid patientId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PatientDto>> GetPatientsAsync(IEnumerable<Guid> patientIds, CancellationToken cancellationToken = default);
    }
}
