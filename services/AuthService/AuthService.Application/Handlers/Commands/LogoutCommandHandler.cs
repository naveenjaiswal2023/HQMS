using AuthService.Application.Commands;
using AuthService.Application.Common.Models;
using AuthService.Domain.Events;
using AuthService.Domain.Identity;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Handlers.Commands
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        {
            try
            {

                await _authService.SignOutAsync();


            }
            catch (Exception ex)
            {
            }
        }
    }
}