using Contracts.Hospitals;
using QueueService.Domain.Interfaces.ExternalServices;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace QueueService.Infrastructure.Services.ExternalServices
{
    public class HospitalServiceClient : IHospitalServiceClient
    {
        private readonly HttpClient _httpClient;

        public HospitalServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HospitalDto> GetHospitalByIdAsync(Guid hospitalId)
        {
            var response = await _httpClient.GetAsync($"/hospitals/{hospitalId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<HospitalDto>();
        }
    }
}