
using QueueService.Application.Common.Interfaces;
using QueueService.Application.Common.Models;
using QueueService.Application.DTOs;
using QueueService.Domain.Enum;
using QueueService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Application.Services
{
    public class QueueQueryService : IQueueQueryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public QueueQueryService(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<List<Result<QueueDetailsDto>>> GetAllQueueDetailsAsync()
        {
            string cacheKey = "all_queue_details";

            // Try cache first
            var cachedData = await _cacheService.GetAsync<List<Result<QueueDetailsDto>>>(cacheKey);
            if (cachedData != null)
                return cachedData;

            // Get from DB
            var queues = await _unitOfWork.QueueItems.GetAllAsync();
            var result = queues.Select(queue =>
                Result<QueueDetailsDto>.Success(new QueueDetailsDto
                {
                    Id = queue.Id,
                    QueueNumber = queue.QueueNumber,
                    Status = (QueueStatus)queue.Status,
                    Position = queue.Position,
                    PatientName = queue.PatientInfo?.Name ?? "Unknown",
                    DoctorName = queue.DoctorInfo?.Name ?? "Unknown",
                    JoinedAt = queue.JoinedAt,
                    CalledAt = queue.CalledAt,
                    CompletedAt = queue.CompletedAt,
                    SkippedAt = queue.SkippedAt,
                    CancelledAt = queue.CancelledAt
                })
            ).ToList();

            // Save in cache
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }

        public async Task<List<Result<QueueDetailsDto>>> GetAllQueueDetailsAsync(Guid hospitalId, Guid departmentId, Guid? doctorId = null, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"queue_details_{hospitalId}_{departmentId}_{doctorId?.ToString() ?? "all"}";

            // Try cache first
            var cachedData = await _cacheService.GetAsync<List<Result<QueueDetailsDto>>>(cacheKey);
            if (cachedData != null)
                return cachedData;

            var today = DateTime.UtcNow.Date;

            // Get from DB with filters
            var queues = await _unitOfWork.QueueItems.GetFilteredQueueItemsAsync(hospitalId, departmentId, doctorId);

            // Filter by today's date
            var filteredQueues = queues.Where(q => q.CreatedAt.Date == today)
                .OrderBy(x => x.HospitalId)
                .ThenBy(x => x.DepartmentId)
                .ThenBy(x => x.DoctorId)
                .ThenBy(x => x.QueueNumber);

            var result = filteredQueues.Select(queue =>
                Result<QueueDetailsDto>.Success(new QueueDetailsDto
                {
                    Id = queue.Id,
                    QueueNumber = queue.QueueNumber,
                    Status = (QueueStatus)queue.Status,
                    Position = queue.Position,
                    PatientName = queue.PatientInfo?.Name ?? "Unknown",
                    DoctorName = queue.DoctorInfo?.Name ?? "Unknown",
                    JoinedAt = queue.JoinedAt,
                    CalledAt = queue.CalledAt,
                    CompletedAt = queue.CompletedAt,
                    SkippedAt = queue.SkippedAt,
                    CancelledAt = queue.CancelledAt
                })
            ).ToList();

            // Save in cache
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }

    }
}
