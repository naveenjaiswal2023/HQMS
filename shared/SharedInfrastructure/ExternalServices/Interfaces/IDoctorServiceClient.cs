using Contracts.Doctors;
using SharedInfrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInfrastructure.ExternalServices.Interfaces
{
    public interface IDoctorServiceClient
    {
        Task<DoctorInfoDto> GetDoctorInfoAsync(Guid doctorId);
        Task<DoctorDto?> GetDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default);
        Task<bool> DoctorExistsAsync(Guid doctorId, CancellationToken cancellationToken = default);
        Task<IEnumerable<DoctorDto>> GetDoctorsByHospitalAsync(Guid hospitalId, CancellationToken cancellationToken = default);
        Task<bool> IsDoctorAvailableAsync(Guid doctorId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
    }
}
