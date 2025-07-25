using Contracts.Appointments;
using QueueService.Application.DTOs;
using QueueService.Domain.Interfaces.ExternalServices;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace QueueService.Infrastructure.Services.ExternalServices
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

        public async Task<List<AppointmentInfoDto>> GetUpcomingAppointmentsAsync(int minutesAhead)
        {
            var from = DateTime.UtcNow;
            var to = from.AddMinutes(minutesAhead);

            var response = await _httpClient.GetAsync($"/api/appointments/upcoming?from={from:o}&to={to:o}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<AppointmentInfoDto>>();
        }

    }
}