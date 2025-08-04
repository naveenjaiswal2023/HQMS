using AuthService.Application.DTOs.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Queries
{
    public class CheckUserExistsQuery : IRequest<CheckUserExistsDto>
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public CheckUserExistsQuery(string email, string phoneNumber)
        {
            Email = email;
            PhoneNumber = phoneNumber;
        }
    }
}
