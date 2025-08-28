using AuthService.Application.Commands;
using AuthService.Application.Common.Models;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.Interfaces;
using AuthService.Domain.Events;
using AuthService.Domain.Identity;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Handlers.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<TokenDto>>
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMediator _mediator;
        private readonly IAuthDbContext _context;

        public LoginCommandHandler(
            IAuthService authService,
            UserManager<ApplicationUser> userManager,
            IMediator mediator,
            IAuthDbContext context)
        {
            _authService = authService;
            _userManager = userManager;
            _mediator = mediator;
            _context = context;
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

            // ✅ Add domain event
            user.AddDomainEvent(
                new UserLoggedInEvent(Guid.Parse(user.Id), user.UserName ?? request.Username, DateTime.UtcNow)
            );


            // ✅ Attach entity so EF starts tracking & SaveChanges will trigger event publishing
            _context.Attach(user);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<TokenDto>.Success(token);
        }

    }
}
