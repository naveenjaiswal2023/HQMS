﻿using HQMS.QueueService.Domain.Interfaces;

namespace HQMS.QueueService.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

        public string? UserName => _httpContextAccessor.HttpContext?.User?.Identity?.Name;
    }
}
