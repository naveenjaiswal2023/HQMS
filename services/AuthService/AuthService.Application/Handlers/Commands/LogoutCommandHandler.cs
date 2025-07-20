using AuthService.Application.Commands;
using AuthService.Application.Common.Models;
using AuthService.Domain.Events;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Handlers.Commands
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<Unit>>
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator _mediator;

        public LogoutCommandHandler(IAuthService authService, IHttpContextAccessor httpContextAccessor, IMediator mediator)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }

        public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                await _authService.SignOutAsync();

                if (!string.IsNullOrEmpty(userId))
                {
                    await _mediator.Publish(new UserLoggedOutEvent(userId, DateTime.UtcNow), cancellationToken);
                }

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure($"Logout failed: {ex.Message}");
            }
        }
    }
}