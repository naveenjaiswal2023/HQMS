using SharedInfrastructure.DTO;
using SharedInfrastructure.ExternalServices.Interfaces;
using System.Net.Http.Json;

namespace SharedInfrastructure.ExternalServices
{
    public class HospitalServiceClient : IHospitalServiceClient
    {
        private readonly HttpClient _httpClient;

        public HospitalServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HospitalDto?> GetHospitalAsync(Guid hospitalId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"/hospitals/{hospitalId}", cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<HospitalDto>(cancellationToken: cancellationToken);
        }

        public async Task<HospitalDto> GetHospitalByIdAsync(Guid hospitalId)
        {
            var response = await _httpClient.GetAsync($"/hospitals/{hospitalId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<HospitalDto>();
        }

        public async Task<IEnumerable<HospitalDto>> GetHospitalsAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync("/hospitals", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<HospitalDto>>(cancellationToken: cancellationToken)
                   ?? Enumerable.Empty<HospitalDto>();
        }

        public async Task<bool> HospitalExistsAsync(Guid hospitalId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"/hospitals/{hospitalId}/exists", cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>(cancellationToken: cancellationToken);
        }
    }
}
