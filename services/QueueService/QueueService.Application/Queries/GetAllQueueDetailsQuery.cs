using MediatR;
using QueueService.Application.Common.Models;
using QueueService.Application.DTOs;
using System;

namespace QueueService.Application.Queries
{
    public class GetAllQueueDetailsQuery : IRequest<PaginatedResult<QueueDetailsDto>>
    {
        public Guid HospitalId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? DoctorId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }
}
