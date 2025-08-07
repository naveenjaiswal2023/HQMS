using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SharedInfrastructure.DTO;
using SharedInfrastructure.ExternalServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedInfrastructure.ExternalServices
{
    public class InternalTokenProvider : IInternalTokenProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<InternalTokenProvider> _logger;

        private string _accessToken = string.Empty;
        private DateTime _expiryTime = DateTime.MinValue;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public InternalTokenProvider(HttpClient httpClient, IConfiguration configuration, ILogger<InternalTokenProvider> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GetTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _expiryTime)
            {
                return _accessToken;
            }

            await _semaphore.WaitAsync();
            try
            {
                if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _expiryTime)
                {
                    return _accessToken;
                }

                var tokenUrl = _configuration["ServiceAuth:TokenUrl"];
                var clientId = _configuration["ServiceAuth:ClientId"];
                var clientSecret = _configuration["ServiceAuth:ClientSecret"];

                if (string.IsNullOrWhiteSpace(tokenUrl) || string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
                {
                    throw new InvalidOperationException("Token URL or credentials are not configured properly.");
                }

                var requestBody = new ClientCredentialDto
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                };

                var response = await _httpClient.PostAsJsonAsync(tokenUrl, requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to acquire token. Status: {response.StatusCode}");
                    throw new HttpRequestException("Failed to acquire token from AuthService.");
                }

                var tokenResult = await response.Content.ReadFromJsonAsync<InternalTokenResultDto>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _accessToken = tokenResult?.Token ?? throw new Exception("Token is null in response.");

                if (tokenResult.Expiration != default)
                {
                    _expiryTime = tokenResult.Expiration.AddSeconds(-60);
                }
                else
                {
                    _expiryTime = DateTime.UtcNow.AddSeconds(tokenResult?.ExpiresIn > 0 ? tokenResult.ExpiresIn - 60 : 240);
                }

                _logger.LogInformation($"[InternalTokenProvider] Token acquired, expires at {_expiryTime:u}");
                return _accessToken;
            }
            finally
            {
                _semaphore.Release();
            }
        }


    }

}
