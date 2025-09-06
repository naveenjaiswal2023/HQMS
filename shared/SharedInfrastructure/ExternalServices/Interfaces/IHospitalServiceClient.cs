
using SharedInfrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInfrastructure.ExternalServices.Interfaces
{
    public interface IHospitalServiceClient
    {
        Task<HospitalDto> GetHospitalByIdAsync(Guid hospitalId);
        Task<HospitalDto?> GetHospitalAsync(Guid hospitalId, CancellationToken cancellationToken = default);
        Task<bool> HospitalExistsAsync(Guid hospitalId, CancellationToken cancellationToken = default);
        Task<IEnumerable<HospitalDto>> GetHospitalsAsync(CancellationToken cancellationToken = default);
    }
}
