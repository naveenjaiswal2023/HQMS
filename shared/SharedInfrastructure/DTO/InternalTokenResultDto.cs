using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedInfrastructure.DTO
{
    public class InternalTokenResultDto
    {
        [JsonPropertyName("accessToken")]
        public string Token { get; set; }

        [JsonPropertyName("expiration")]
        public DateTime Expiration { get; set; }
        
        public int ExpiresIn { get; set; }
    }

}

