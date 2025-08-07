
using Contracts.Patients;
using SharedInfrastructure.ExternalServices.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Json;
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

        public async Task<PatientInfoDto> GetPatientInfoAsync(Guid patientId)
        {
            var response = await _httpClient.GetAsync($"/patients/{patientId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PatientInfoDto>();
        }
    }
}
