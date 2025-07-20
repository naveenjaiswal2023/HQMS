using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Functions.Models
{
    public class LoginEventDto
    {
        public string UserId { get; set; }
        public DateTime LoginTime { get; set; }
        public string IpAddress { get; set; }
    }
}
