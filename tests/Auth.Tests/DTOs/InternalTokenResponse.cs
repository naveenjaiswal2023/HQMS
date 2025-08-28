using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Tests.DTOs
{
    public class InternalTokenResponse
    {
        public string AccessToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
