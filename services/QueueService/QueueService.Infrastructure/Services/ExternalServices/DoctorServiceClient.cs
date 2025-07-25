using Contracts.Doctors;
using QueueService.Domain.Interfaces.ExternalServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Infrastructure.Services.ExternalServices
{
    public class DoctorServiceClient : IDoctorServiceClient
    {
        private readonly HttpClient _httpClient;

        public DoctorServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DoctorInfoDto> GetDoctorInfoAsync(Guid doctorId)
        {
            var response = await _httpClient.GetAsync($"/doctors/{doctorId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DoctorInfoDto>();
        }
    }
}
