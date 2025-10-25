using Contracts.Patients;
using SharedInfrastructure.DTO;
using SharedInfrastructure.ExternalServices.Interfaces;
using SharedInfrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
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
            var response = await _httpClient.GetAsync($"/api/patients/{patientId}", cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                // Log or handle error content here
                throw new ApplicationException($"API returned error {response.StatusCode}: {errorContent}");
            }

            var contentString = await response.Content.ReadAsStringAsync(cancellationToken);

            //bool result;
            try
            {
                var result = JsonSerializer.Deserialize<Result<PatientDto>>(contentString);
                if (result.Succeeded)
                    return true;
                else
                    return false;
            }
            catch (JsonException)
            {
                throw new InvalidOperationException("Response content is not a valid boolean.");
            }


            //return result;
        }

    }
}
