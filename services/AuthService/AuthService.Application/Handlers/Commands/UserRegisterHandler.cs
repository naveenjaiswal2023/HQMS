using AuthService.Application.Commands;
using AuthService.Application.Common.Models;
using AuthService.Domain.Events;
using AuthService.Domain.Identity;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace AuthService.Application.Handlers.Commands
{
    public class UserRegisterHandler : IRequestHandler<UserRegisterCommand, Result<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public UserRegisterHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
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

            // Add role
            if (!await _roleManager.RoleExistsAsync(request.Role))
                await _roleManager.CreateAsync(new ApplicationRole(request.Role));

            await _userManager.AddToRoleAsync(user, request.Role);

            // Generate token for confirmation
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var confirmUrl = $"https://your-client.com/confirm-email?userId={user.Id}&token={encodedToken}";

            // ✅ Attach the user entity to UnitOfWork so events are tracked
            //_unitOfWork.AttachEntity(user);

            // ✅ Add domain event (now _unitOfWork.SaveAsync will catch it)
            user.AddDomainEvent(new UserRegisteredEvent(
                Guid.Parse(user.Id),
                user.Email!,
                user.UserName!,
                request.Role,
                confirmUrl,
                DateTime.UtcNow
            ));

            // ✅ This will now dispatch domain events
            await _unitOfWork.SaveAsync(cancellationToken);

            return Result<string>.Success("User registered. Please confirm your email.");
        }
    }
}
