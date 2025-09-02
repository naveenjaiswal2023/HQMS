//using Microsoft.Extensions.Logging;
//using PaymentService.Application.Commands;
//using PaymentService.Application.Interfaces;
//using PaymentService.Domain.Enums;
//using PaymentService.Domain.Interfaces;
//using Stripe;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace PaymentService.Application.Services
//{
//    public class PaymentServiceClient : IPaymentServiceClient
//    {
//        private readonly HttpClient _httpClient;
//        private readonly ILogger<PaymentServiceClient> _logger;

//        public PaymentServiceClient(HttpClient httpClient, ILogger<PaymentServiceClient> logger)
//        {
//            _httpClient = httpClient;
//            _logger = logger;
//        }

//        public async Task<InitiatePaymentResult> InitiateRegistrationPaymentAsync(Guid patientId, string feeType, Stripe.PaymentMethod paymentMethod, string email, string phone)
//        {
//            try
//            {
//                // Convert Stripe PaymentMethod to your domain enum
//                var domainPaymentMethod = ConvertStripePaymentMethodToDomain(paymentMethod);
//                var request = new InitiateRegistrationPaymentCommand(patientId, feeType, domainPaymentMethod, email, phone);
//                var jsonContent = JsonSerializer.Serialize(request);
//                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

//                var response = await _httpClient.PostAsync("api/payments/initiate-registration", content);
//                response.EnsureSuccessStatusCode();

//                var responseContent = await response.Content.ReadAsStringAsync();
//                return JsonSerializer.Deserialize<InitiatePaymentResult>(responseContent, new JsonSerializerOptions
//                {
//                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Failed to initiate registration payment for patient {PatientId}", patientId);
//                throw;
//            }
//        }

//        public async Task<PaymentVerificationResult> VerifyPaymentAsync(string transactionId)
//        {
//            try
//            {
//                var response = await _httpClient.PostAsync($"api/payments/verify/{transactionId}", null);
//                response.EnsureSuccessStatusCode();

//                var responseContent = await response.Content.ReadAsStringAsync();
//                return JsonSerializer.Deserialize<PaymentVerificationResult>(responseContent, new JsonSerializerOptions
//                {
//                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Failed to verify payment for transaction {TransactionId}", transactionId);
//                throw;
//            }
//        }

//        private PaymentService.Domain.Enums.PaymentMethod ConvertStripePaymentMethodToDomain(Stripe.PaymentMethod stripePaymentMethod)
//        {
//            // Convert Stripe PaymentMethod to your domain enum
//            return stripePaymentMethod?.Type?.ToLowerInvariant() switch
//            {
//                "card" when IsDebitCard(stripePaymentMethod) => PaymentService.Domain.Enums.PaymentMethod.DebitCard,
//                "card" => PaymentService.Domain.Enums.PaymentMethod.CreditCard,
//                "us_bank_account" => PaymentService.Domain.Enums.PaymentMethod.NetBanking,
//                "bacs_debit" => PaymentService.Domain.Enums.PaymentMethod.NetBanking,
//                "sepa_debit" => PaymentService.Domain.Enums.PaymentMethod.NetBanking,
//                "upi" => PaymentService.Domain.Enums.PaymentMethod.UPI,
//                "wallet" => PaymentService.Domain.Enums.PaymentMethod.Wallet,
//                "link" => PaymentService.Domain.Enums.PaymentMethod.Wallet,
//                _ => PaymentService.Domain.Enums.PaymentMethod.CreditCard // Default fallback
//            };
//        }

//        private bool IsDebitCard(Stripe.PaymentMethod stripePaymentMethod)
//        {
//            // Check if the card is a debit card based on Stripe's card details
//            return stripePaymentMethod?.Card?.Funding?.ToLowerInvariant() == "debit";
//        }
//    }
//}