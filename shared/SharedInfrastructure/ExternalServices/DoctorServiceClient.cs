using Contracts.Doctors;
using SharedInfrastructure.DTO;
using SharedInfrastructure.ExternalServices.Interfaces;
using System.Net.Http.Json;

namespace SharedInfrastructure.ExternalServices
{
    public class DoctorServiceClient : IDoctorServiceClient
    {
        private readonly HttpClient _httpClient;

        public DoctorServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> DoctorExistsAsync(Guid doctorId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"/doctors/{doctorId}/exists", cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>(cancellationToken: cancellationToken);
        }

        public async Task<DoctorDto?> GetDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"/doctors/{doctorId}", cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DoctorDto>(cancellationToken: cancellationToken);
        }

        public async Task<DoctorInfoDto> GetDoctorInfoAsync(Guid doctorId)
        {
            var response = await _httpClient.GetAsync($"/doctors/{doctorId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DoctorInfoDto>();
        }

        public async Task<IEnumerable<DoctorDto>> GetDoctorsByHospitalAsync(Guid hospitalId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"/hospitals/{hospitalId}/doctors", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<DoctorDto>>(cancellationToken: cancellationToken)
                   ?? Enumerable.Empty<DoctorDto>();
        }

        public async Task<bool> IsDoctorAvailableAsync(Guid doctorId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(
                $"/doctors/{doctorId}/availability?startTime={startTime:o}&endTime={endTime:o}",
                cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>(cancellationToken: cancellationToken);
        }
    }
}
