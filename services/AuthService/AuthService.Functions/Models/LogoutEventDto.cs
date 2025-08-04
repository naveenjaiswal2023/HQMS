using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Functions.Models
{
    public class LogoutEventDto
    {
        public string UserId { get; set; }
        public DateTime LoggedOutAt { get; set; }
    }
}
