using PaymentService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Infrastructure.Services
{
    public class PatientRegistrationService : IPatientRegistrationService
    {
        private readonly HttpClient _httpClient;

        public PatientRegistrationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CompleteRegistrationAsync(Guid patientId)
        {
            var response = await _httpClient.PostAsync($"api/patients/{patientId}/complete-registration", null);
            response.EnsureSuccessStatusCode();
        }
    }
}
