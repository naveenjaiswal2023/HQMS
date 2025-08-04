using AuthService.Application.DTOs.Auth;
using AuthService.Application.Queries;
using AuthService.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Handlers.Queries
{
    public class CheckUserExistsQueryHandler : IRequestHandler<CheckUserExistsQuery, CheckUserExistsDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckUserExistsQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CheckUserExistsDto> Handle(CheckUserExistsQuery request, CancellationToken cancellationToken)
        {
            var emailExists = await _userManager.FindByEmailAsync(request.Email) != null;
            var phoneExists = await _userManager.Users.AnyAsync(x => x.PhoneNumber == request.PhoneNumber, cancellationToken);

            return new CheckUserExistsDto
            {
                Email = emailExists ? request.Email : null,
                PhoneNumber = phoneExists ? request.PhoneNumber : null
            };
        }
    }
}
