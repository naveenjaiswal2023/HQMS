using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SharedInfrastructure.Common.Extensions
{
    public static class HttpClientExtensions
    {
        public static void AddBearerTokenFromHttpContext(this HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            var token = httpContextAccessor.HttpContext?.Request
                ?.Headers["Authorization"].ToString()?.Replace("Bearer ", "");

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
