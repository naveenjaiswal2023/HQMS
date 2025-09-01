using Microsoft.Extensions.Logging;
using PatientService.Application.Commands;
using PatientService.Application.Interfaces;
using PatientService.Domain.Enums;
using PatientService.Domain.Interfaces;
using Stripe.V2;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PatientService.Application.Services
{
    public class PaymentServiceClient : IPaymentServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PaymentServiceClient> _logger;

        public PaymentServiceClient(HttpClient httpClient, ILogger<PaymentServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // Corrected: async + using domain PaymentMethod
        public async Task<InitiatePaymentResult> InitiateRegistrationPaymentAsync(
            Guid patientId,
            string feeType,
            decimal amount,
            PaymentMethod paymentMethod, // PatientService.Domain.Enums
            string PayerName,
            string email,
            string phone)
        {
            //var request = new InitiateRegistrationPaymentCommand(patientId, feeType, paymentMethod, email, phone);
            var request = new InitiateRegistrationPaymentCommand(
                patientId,
                feeType,
                amount,        // e.g., 500m
                "INR",         // or "USD"
                paymentMethod,
                PayerName,
                email,
                phone
            );


            var jsonContent = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/payments/initiate-registration", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<InitiatePaymentResult>(
                responseContent,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );
        }

        

        public Task<InitiatePaymentResult> InitiateRegistrationPaymentAsync(Guid patientId, string registrationFeeType, PaymentMethod paymentMethod, string? email, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentVerificationResult> VerifyPaymentAsync(string transactionId)
        {
            var response = await _httpClient.PostAsync($"api/payments/verify/{transactionId}", null);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<PaymentVerificationResult>(
                responseContent,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );
        }
    }
}
