using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs.Auth
{
    public class CheckUserExistsDto
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
