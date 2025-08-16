using MediatR;
using QueueService.Application.Common.Interfaces;
using QueueService.Application.Common.Models;
using QueueService.Application.DTOs;
using QueueService.Application.Queries;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService.Application.Handlers.Queries
{
    public class GetAllQueueDetailsQueryHandler
        : IRequestHandler<GetAllQueueDetailsQuery, PaginatedResult<QueueDetailsDto>>
    {
        private readonly IQueueQueryService _queueQueryService;

        public GetAllQueueDetailsQueryHandler(IQueueQueryService queueQueryService)
        {
            _queueQueryService = queueQueryService;
        }

        public async Task<PaginatedResult<QueueDetailsDto>> Handle(
            GetAllQueueDetailsQuery request,
            CancellationToken cancellationToken)
        {
            // Get filtered queue details from the service
            var allItems = await _queueQueryService.GetAllQueueDetailsAsync(
            request.HospitalId,
            request.DepartmentId,
            request.DoctorId,
            cancellationToken);

            // Extract successful results
            var queueDetailsList = allItems
                .Where(r => r.Succeeded && r.Data != null)
                .Select(r => r.Data)
                .ToList();

            // Apply pagination
            var totalCount = queueDetailsList.Count;
            var pagedData = queueDetailsList
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Return paginated result
            return PaginatedResult<QueueDetailsDto>.Success(
                pagedData,
                request.PageNumber,
                request.PageSize,
                totalCount);
        }
    }
}
