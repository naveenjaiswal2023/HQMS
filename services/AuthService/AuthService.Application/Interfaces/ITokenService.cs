using AuthService.Application.DTOs.Auth;
using AuthService.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Interfaces
{
    public interface ITokenService
    {
        Task<TokenDto> GenerateToken(ApplicationUser user);
    }
}
