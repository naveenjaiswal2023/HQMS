using QueueService.Application.Common.Models;
using QueueService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.Common.Interfaces
{
    public interface IQueueQueryService
    {
        Task<List<Result<QueueDetailsDto>>> GetAllQueueDetailsAsync();
        Task<List<Result<QueueDetailsDto>>> GetAllQueueDetailsAsync(Guid hospitalId, Guid departmentId, Guid? doctorId = null, CancellationToken cancellationToken = default);
    }

}
