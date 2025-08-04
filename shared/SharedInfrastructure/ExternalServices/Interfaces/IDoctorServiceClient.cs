using Contracts.Doctors;
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
    }
}
