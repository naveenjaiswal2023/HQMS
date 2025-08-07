using Microsoft.AspNetCore.Http;
using SharedInfrastructure.ExternalServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SharedInfrastructure.Http
{
    public class AuthenticatedHttpClientHandler : DelegatingHandler
    {
        private readonly IInternalTokenProvider _tokenProvider;

        public AuthenticatedHttpClientHandler(IInternalTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenProvider.GetTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }
    }

}
