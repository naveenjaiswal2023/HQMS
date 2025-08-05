using Contracts.Doctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Domain.Interfaces.ExternalServices
{
    public interface IDoctorServiceClient
    {
        Task<DoctorInfoDto> GetDoctorInfoAsync(Guid doctorId);
    }
}
