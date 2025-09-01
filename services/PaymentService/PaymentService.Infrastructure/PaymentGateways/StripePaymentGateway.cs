using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Enums;
using PaymentService.Domain.Interfaces;
using PaymentService.Domain.Models.Payments;
//using PaymentService.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PaymentService.Infrastructure.PaymentGateways
{
    public class StripePaymentGateway : IPaymentGateway
    {
        private readonly HttpClient _httpClient;
        private readonly StripeConfig _config;
        private readonly ILogger<StripePaymentGateway> _logger;

        public StripePaymentGateway(HttpClient httpClient, IOptions<StripeConfig> config, ILogger<StripePaymentGateway> logger)
        {
            _httpClient = httpClient;
            _config = config.Value;
            _logger = logger;

            // Set up authentication for Stripe
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.SecretKey);
            _httpClient.BaseAddress = new Uri("https://api.stripe.com/v1/");
        }

        public async Task<PaymentInitiationResponse> InitiatePaymentAsync(PaymentInitiationRequest request)
        {
            try
            {
                // Create Payment Intent in Stripe
                var payload = new Dictionary<string, string>
                {
                    ["amount"] = ((int)(request.Amount * 100)).ToString(), // Convert to cents
                    ["currency"] = request.Currency.ToLower(),
                    ["description"] = request.Description,
                    ["receipt_email"] = request.CustomerEmail,
                    ["metadata[transaction_id]"] = request.TransactionId,
                    ["metadata[customer_phone]"] = request.CustomerPhone ?? "",
                    ["automatic_payment_methods[enabled]"] = "true",
                    ["confirmation_method"] = "manual",
                    ["confirm"] = "false"
                };

                // Add customer information if available
                if (!string.IsNullOrEmpty(request.CustomerEmail))
                {
                    payload["receipt_email"] = request.CustomerEmail;
                }

                var formContent = new FormUrlEncodedContent(payload);

                var response = await _httpClient.PostAsync("payment_intents", formContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var paymentIntent = JsonSerializer.Deserialize<StripePaymentIntentResponse>(responseContent);
                    var paymentUrl = GeneratePaymentUrl(paymentIntent.ClientSecret, request);

                    return new PaymentInitiationResponse(
                        true,
                        paymentUrl,
                        paymentIntent.Id,
                        null
                    );
                }
                else
                {
                    _logger.LogError("Stripe payment intent creation failed: {Response}", responseContent);
                    var errorResponse = JsonSerializer.Deserialize<StripeErrorResponse>(responseContent);
                    return new PaymentInitiationResponse(false, null, null, errorResponse?.Error?.Message ?? "Payment initiation failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating payment with Stripe");
                return new PaymentInitiationResponse(false, null, null, ex.Message);
            }
        }

        public async Task<PaymentVerificationResponse> VerifyPaymentAsync(string transactionId)
        {
            try
            {
                // Get payment intent details from Stripe
                var response = await _httpClient.GetAsync($"payment_intents/{transactionId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var paymentIntent = JsonSerializer.Deserialize<StripePaymentIntentResponse>(responseContent);

                    var status = paymentIntent.Status switch
                    {
                        "succeeded" => PaymentStatus.Success,
                        "canceled" => PaymentStatus.Failed,
                        "processing" => PaymentStatus.Processing,
                        "requires_payment_method" => PaymentStatus.Failed,
                        "requires_confirmation" => PaymentStatus.Pending,
                        "requires_action" => PaymentStatus.Pending,
                        "requires_capture" => PaymentStatus.Processing,
                        _ => PaymentStatus.Pending
                    };

                    return new PaymentVerificationResponse(
                        status == PaymentStatus.Success,
                        status,
                        responseContent,
                        paymentIntent.LastPaymentError?.Message
                    );
                }
                else
                {
                    _logger.LogError("Stripe payment verification failed: {Response}", responseContent);
                    var errorResponse = JsonSerializer.Deserialize<StripeErrorResponse>(responseContent);
                    return new PaymentVerificationResponse(false, PaymentStatus.Failed, responseContent, errorResponse?.Error?.Message ?? "Payment verification failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying payment for transaction {TransactionId}", transactionId);
                return new PaymentVerificationResponse(false, PaymentStatus.Failed, null, ex.Message);
            }
        }

        public async Task<RefundResponse> ProcessRefundAsync(RefundRequest request)
        {
            try
            {
                var payload = new Dictionary<string, string>
                {
                    ["payment_intent"] = request.OriginalTransactionId,
                    ["reason"] = "requested_by_customer"
                };

                // Add amount if partial refund
                if (request.RefundAmount > 0)
                {
                    payload["amount"] = ((int)(request.RefundAmount * 100)).ToString(); // Convert to cents
                }

                // Add reason metadata
                if (!string.IsNullOrEmpty(request.Reason))
                {
                    payload["metadata[reason]"] = request.Reason;
                }

                var formContent = new FormUrlEncodedContent(payload);

                var response = await _httpClient.PostAsync("refunds", formContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var refundResponse = JsonSerializer.Deserialize<StripeRefundResponse>(responseContent);
                    return new RefundResponse(true, refundResponse.Id, null);
                }
                else
                {
                    _logger.LogError("Stripe refund failed: {Response}", responseContent);
                    var errorResponse = JsonSerializer.Deserialize<StripeErrorResponse>(responseContent);
                    return new RefundResponse(false, null, errorResponse?.Error?.Message ?? "Refund failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund with Stripe");
                return new RefundResponse(false, null, ex.Message);
            }
        }

        private string GeneratePaymentUrl(string clientSecret, PaymentInitiationRequest request)
        {
            // Return URL for frontend to complete payment or the client secret
            // In most cases, you'll return the client_secret to your frontend
            return $"/payment/stripe-checkout?client_secret={clientSecret}&amount={request.Amount}&currency={request.Currency}";
        }

        // Helper method to validate Stripe configuration
        public bool IsConfigured()
        {
            return !string.IsNullOrEmpty(_config.SecretKey) &&
                   !string.IsNullOrEmpty(_config.PublishableKey) &&
                   _config.SecretKey.StartsWith("sk_");
        }

        // Helper method to get publishable key for frontend
        public string GetPublishableKey()
        {
            return _config.PublishableKey;
        }
    }
}