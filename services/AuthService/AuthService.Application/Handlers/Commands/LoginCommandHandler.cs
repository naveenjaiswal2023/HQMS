using AuthService.Application.Commands;
using AuthService.Application.Common.Models;
using AuthService.Application.DTOs.Auth;
using AuthService.Domain.Events;
using AuthService.Domain.Identity;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Handlers.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<TokenDto>>
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMediator _mediator;

        public LoginCommandHandler(IAuthService authService, UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            _authService = authService;
            _userManager = userManager;
            _mediator = mediator;
        }

        public async Task<Result<TokenDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                return Result<TokenDto>.Failure("User not found.");

            var signInResult = await _authService.PasswordSignInAsync(request.Username, request.Password, request.RememberMe);
            if (!signInResult.Succeeded)
                return Result<TokenDto>.Failure("Invalid credentials.");

            var token = await _authService.GenerateJwtTokenAsync(user);

            // Optional: Trigger domain event after successful login
            await _mediator.Publish(new UserLoggedInEvent(user.Id, DateTime.UtcNow));

            return Result<TokenDto>.Success(token);
        }
    }
}
