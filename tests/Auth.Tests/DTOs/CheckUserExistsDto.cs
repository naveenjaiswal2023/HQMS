using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Tests.DTOs
{
    public class CheckUserExistsDto
    {
        public bool Email { get; set; }
        public bool Phone { get; set; }
    }
}
