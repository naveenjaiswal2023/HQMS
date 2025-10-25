using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using SharedInfrastructure.ExternalServices.Interfaces;

namespace SharedInfrastructure.Http
{
    public class AuthenticatedHttpClientHandler : DelegatingHandler
    {
        private readonly IInternalTokenProvider _tokenProvider;
        private readonly IMemoryCache _memoryCache;
        private const string TokenCacheKey = "AuthToken";

        public AuthenticatedHttpClientHandler(IInternalTokenProvider tokenProvider, IMemoryCache memoryCache)
        {
            _tokenProvider = tokenProvider;
            _memoryCache = memoryCache;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string token = _memoryCache.Get<string>(TokenCacheKey);

            if (string.IsNullOrEmpty(token))
            {
                token = await _tokenProvider.GetTokenAsync();

                // Set token in cache with expiration - example 60 minutes expiry
                _memoryCache.Set(TokenCacheKey, token, TimeSpan.FromMinutes(60));
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
