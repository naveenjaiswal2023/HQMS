using Contracts.Hospitals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Domain.Interfaces.ExternalServices
{
    public interface IHospitalServiceClient
    {
        Task<HospitalDto> GetHospitalByIdAsync(Guid hospitalId);
    }
}
