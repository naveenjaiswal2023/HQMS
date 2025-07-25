using AuthService.Application.Commands;
using AuthService.Application.Common.Models;
using AuthService.Domain.Events;
using AuthService.Domain.Identity;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Web;

namespace AuthService.Application.Handlers.Commands
{
    public class UserRegisterHandler : IRequestHandler<UserRegisterCommand, Result<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMediator _mediator;
        private readonly IEmailSender _emailSender;

        public UserRegisterHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IMediator mediator,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mediator = mediator;
            _emailSender = emailSender;
        }

        public async Task<Result<string>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return Result<string>.Failure("User already exists with this email.");

            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                EmailConfirmed = false
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
                return Result<string>.Failure(createResult.Errors.Select(e => e.Description).ToArray());

            if (!await _roleManager.RoleExistsAsync(request.Role))
                await _roleManager.CreateAsync(new ApplicationRole(request.Role));

            await _userManager.AddToRoleAsync(user, request.Role);

            // Send confirmation email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var confirmUrl = $"https://your-client.com/confirm-email?userId={user.Id}&token={encodedToken}";

            var message = $"<p>Hello {user.UserName},</p><p>Click the link to confirm your email: <a href='{confirmUrl}'>Confirm Email</a></p>";
            //await _emailSender.SendEmailAsync(user.Email, "Confirm your email", message);

            // Publish domain event
            var userRegisteredEvent = new UserRegisteredEvent(
                user.Id,
                user.Email,
                user.UserName,
                request.Role,
                DateTime.UtcNow
            );   
            await _mediator.Publish(userRegisteredEvent);

            return Result<string>.Success("User registered. Please confirm your email.");
        }
    }
}