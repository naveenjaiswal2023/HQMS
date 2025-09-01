using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Enums;
using PaymentService.Domain.Interfaces;
using PaymentService.Domain.Models.Payments;
using PaymentService.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PaymentService.Infrastructure.PaymentGateways
{
    public class RazorpayPaymentGateway : IPaymentGateway
    {
        private readonly HttpClient _httpClient;
        private readonly RazorpayConfig _config;
        private readonly ILogger<RazorpayPaymentGateway> _logger;

        public RazorpayPaymentGateway(HttpClient httpClient, IOptions<RazorpayConfig> config, ILogger<RazorpayPaymentGateway> logger)
        {
            _httpClient = httpClient;
            _config = config.Value;
            _logger = logger;

            // Set up authentication
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_config.KeyId}:{_config.KeySecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        }

        public async Task<PaymentInitiationResponse> InitiatePaymentAsync(PaymentInitiationRequest request)
        {
            try
            {
                var payload = new
                {
                    amount = (int)(request.Amount * 100), // Convert to paisa
                    currency = request.Currency,
                    receipt = request.TransactionId,
                    description = request.Description,
                    customer = new
                    {
                        email = request.CustomerEmail,
                        contact = request.CustomerPhone
                    },
                    notify = new
                    {
                        sms = true,
                        email = true
                    }
                };

                var jsonContent = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://api.razorpay.com/v1/orders", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var orderResponse = JsonSerializer.Deserialize<RazorpayOrderResponse>(responseContent);
                    var paymentUrl = GeneratePaymentUrl(orderResponse.Id, request);

                    return new PaymentInitiationResponse(
                        true,
                        paymentUrl,
                        orderResponse.Id,
                        null
                    );
                }
                else
                {
                    _logger.LogError("Razorpay order creation failed: {Response}", responseContent);
                    return new PaymentInitiationResponse(false, null, null, "Payment initiation failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating payment");
                return new PaymentInitiationResponse(false, null, null, ex.Message);
            }
        }

        public async Task<PaymentVerificationResponse> VerifyPaymentAsync(string transactionId)
        {
            try
            {
                // Get payment details from Razorpay
                var response = await _httpClient.GetAsync($"https://api.razorpay.com/v1/orders/{transactionId}/payments");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var paymentsResponse = JsonSerializer.Deserialize<RazorpayPaymentsResponse>(responseContent);
                    var latestPayment = paymentsResponse.Items?.FirstOrDefault();

                    if (latestPayment != null)
                    {
                        var status = latestPayment.Status switch
                        {
                            "captured" => PaymentStatus.Success,
                            "failed" => PaymentStatus.Failed,
                            "authorized" => PaymentStatus.Processing,
                            _ => PaymentStatus.Pending
                        };

                        return new PaymentVerificationResponse(
                            status == PaymentStatus.Success,
                            status,
                            responseContent,
                            latestPayment.ErrorDescription
                        );
                    }
                }

                return new PaymentVerificationResponse(false, PaymentStatus.Failed, responseContent, "Payment not found");
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
                var payload = new
                {
                    amount = (int)(request.RefundAmount * 100), // Convert to paisa
                    notes = new { reason = request.Reason }
                };

                var jsonContent = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"https://api.razorpay.com/v1/payments/{request.OriginalTransactionId}/refund", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var refundResponse = JsonSerializer.Deserialize<RazorpayRefundResponse>(responseContent);
                    return new RefundResponse(true, refundResponse.Id, null);
                }

                return new RefundResponse(false, null, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund");
                return new RefundResponse(false, null, ex.Message);
            }
        }

        private string GeneratePaymentUrl(string orderId, PaymentInitiationRequest request)
        {
            // Generate Razorpay Checkout URL or return order ID for frontend integration
            return $"/payment/checkout?order_id={orderId}&amount={request.Amount}&currency={request.Currency}";
        }
    }
}
