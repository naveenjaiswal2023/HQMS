using Contracts.Patients;
using SharedInfrastructure.DTO;
using SharedInfrastructure.ExternalServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SharedInfrastructure.ExternalServices
{
    public class PatientServiceClient : IPatientServiceClient
    {
        private readonly HttpClient _httpClient;

        public PatientServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PatientDto?> GetPatientAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"/patients/{patientId}", cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PatientDto>(cancellationToken: cancellationToken);
        }

        public async Task<PatientInfoDto> GetPatientInfoAsync(Guid patientId)
        {
            var response = await _httpClient.GetAsync($"/patients/{patientId}/info");

            response.EnsureSuccessStatusCode();

            var dto = await response.Content.ReadFromJsonAsync<PatientInfoDto>();

            if (dto == null)
                throw new InvalidOperationException($"Patient info for {patientId} could not be retrieved.");

            return dto;
        }

        public async Task<IEnumerable<PatientDto>> GetPatientsAsync(IEnumerable<Guid> patientIds, CancellationToken cancellationToken = default)
        {
            // Send list of patient IDs as JSON POST body
            var response = await _httpClient.PostAsJsonAsync("/patients/batch", patientIds, cancellationToken);

            response.EnsureSuccessStatusCode();

            var patients = await response.Content.ReadFromJsonAsync<IEnumerable<PatientDto>>(cancellationToken: cancellationToken);

            return patients ?? Array.Empty<PatientDto>();
        }

        public async Task<bool> PatientExistsAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"/patients/{patientId}/exists", cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<bool>(cancellationToken: cancellationToken);

            return result;
        }
    }
}
