using Contracts.Appointments;
using SharedInfrastructure.ExternalServices.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedInfrastructure.ExternalServices
{
    public class AppointmentServiceClient : IAppointmentServiceClient
    {
        private readonly HttpClient _httpClient;

        public AppointmentServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AppointmentInfoDto> GetAppointmentInfoAsync(Guid appointmentId)
        {
            var response = await _httpClient.GetAsync($"/appointments/{appointmentId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppointmentInfoDto>();
        }

        public async Task<List<AppointmentInfoDto>> GetUpcomingAppointmentsAsync(DateTime fromTime, DateTime toTime)
        {
            var url = $"/api/Appointments/GetUpcomingAppointments?from={fromTime:o}&to={toTime:o}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<AppointmentInfoDto>>();
        }

        public async Task UpdateAppointmentStatusAsync(Guid appointmentId, bool status)
        {
            var requestUrl = $"/api/Appointments/{appointmentId}/UpdateStatus";
            var content = new StringContent(JsonSerializer.Serialize(status), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(requestUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update appointment status: {response.StatusCode}, {error}");
            }
        }
    }
}