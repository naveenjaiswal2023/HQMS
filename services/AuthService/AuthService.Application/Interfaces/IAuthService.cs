using AuthService.Application.DTOs.Auth;
using AuthService.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<SignInResult> PasswordSignInAsync(string username, string password, bool rememberMe);
        Task<IdentityResult> RegisterAsync(Identity.ApplicationUser user, string password);
        Task<TokenDto> GenerateJwtTokenAsync(Identity.ApplicationUser user);
        Task<ApplicationUser?> FindByNameAsync(string username);
        Task SignOutAsync();
    }
}
