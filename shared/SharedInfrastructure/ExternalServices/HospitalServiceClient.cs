using Contracts.Hospitals;
using SharedInfrastructure.ExternalServices.Interfaces;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SharedInfrastructures.ExternalServices
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