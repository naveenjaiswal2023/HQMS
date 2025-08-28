using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Tests.DTOs
{
    public class ClientCredentialsRequest
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
