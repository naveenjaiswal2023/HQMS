using Microsoft.Extensions.Logging;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Enums;
using PaymentService.Domain.Interfaces;
using PaymentService.Domain.Models.Payments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentService.Infrastructure.PaymentGateways
{
    public class PaymentGatewayManager
    {
        private readonly RazorpayPaymentGateway _razorpayGateway;
        private readonly StripePaymentGateway _stripeGateway;
        private readonly ILogger<PaymentGatewayManager> _logger;

        public PaymentGatewayManager(
            RazorpayPaymentGateway razorpayGateway,
            StripePaymentGateway stripeGateway,
            ILogger<PaymentGatewayManager> logger)
        {
            _razorpayGateway = razorpayGateway;
            _stripeGateway = stripeGateway;
            _logger = logger;

            _logger.LogInformation("Payment Gateway Manager initialized with Stripe and Razorpay");
        }

        // Stripe Methods
        public async Task<PaymentInitiationResponse> InitiateStripePaymentAsync(PaymentInitiationRequest request)
        {
            try
            {
                _logger.LogInformation("Initiating Stripe payment for amount {Amount} {Currency}", request.Amount, request.Currency);
                var response = await _stripeGateway.InitiatePaymentAsync(request);

                if (response.IsSuccess)
                {
                    _logger.LogInformation("Stripe payment initiation successful");
                }
                else
                {
                    _logger.LogWarning("Stripe payment initiation failed: {Error}", response.ErrorMessage);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating Stripe payment");
                return new PaymentInitiationResponse(false, null, null, "Stripe payment error: " + ex.Message);
            }
        }

        public async Task<PaymentVerificationResponse> VerifyStripePaymentAsync(string transactionId)
        {
            try
            {
                _logger.LogInformation("Verifying Stripe payment {TransactionId}", transactionId);
                return await _stripeGateway.VerifyPaymentAsync(transactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying Stripe payment {TransactionId}", transactionId);
                return new PaymentVerificationResponse(false, PaymentStatus.Failed, null, "Stripe verification error: " + ex.Message);
            }
        }

        public async Task<RefundResponse> ProcessStripeRefundAsync(RefundRequest request)
        {
            try
            {
                _logger.LogInformation("Processing Stripe refund for {TransactionId}, Amount: {Amount}",
                    request.OriginalTransactionId, request.RefundAmount);
                return await _stripeGateway.ProcessRefundAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Stripe refund for {TransactionId}", request.OriginalTransactionId);
                return new RefundResponse(false, null, "Stripe refund error: " + ex.Message);
            }
        }

        // Razorpay Methods
        public async Task<PaymentInitiationResponse> InitiateRazorpayPaymentAsync(PaymentInitiationRequest request)
        {
            try
            {
                _logger.LogInformation("Initiating Razorpay payment for amount {Amount} {Currency}", request.Amount, request.Currency);
                var response = await _razorpayGateway.InitiatePaymentAsync(request);

                if (response.IsSuccess)
                {
                    _logger.LogInformation("Razorpay payment initiation successful");
                }
                else
                {
                    _logger.LogWarning("Razorpay payment initiation failed: {Error}", response.ErrorMessage);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating Razorpay payment");
                return new PaymentInitiationResponse(false, null, null, "Razorpay payment error: " + ex.Message);
            }
        }

        public async Task<PaymentVerificationResponse> VerifyRazorpayPaymentAsync(string transactionId)
        {
            try
            {
                _logger.LogInformation("Verifying Razorpay payment {TransactionId}", transactionId);
                return await _razorpayGateway.VerifyPaymentAsync(transactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying Razorpay payment {TransactionId}", transactionId);
                return new PaymentVerificationResponse(false, PaymentStatus.Failed, null, "Razorpay verification error: " + ex.Message);
            }
        }

        public async Task<RefundResponse> ProcessRazorpayRefundAsync(RefundRequest request)
        {
            try
            {
                _logger.LogInformation("Processing Razorpay refund for {TransactionId}, Amount: {Amount}",
                    request.OriginalTransactionId, request.RefundAmount);
                return await _razorpayGateway.ProcessRefundAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Razorpay refund for {TransactionId}", request.OriginalTransactionId);
                return new RefundResponse(false, null, "Razorpay refund error: " + ex.Message);
            }
        }

        // Generic Methods with Gateway Type Parameter
        public async Task<PaymentInitiationResponse> InitiatePaymentAsync(PaymentInitiationRequest request, PaymentGatewayType gatewayType)
        {
            return gatewayType switch
            {
                PaymentGatewayType.Stripe => await InitiateStripePaymentAsync(request),
                PaymentGatewayType.Razorpay => await InitiateRazorpayPaymentAsync(request),
                _ => new PaymentInitiationResponse(false, null, null, $"Gateway {gatewayType} is not supported")
            };
        }

        public async Task<PaymentVerificationResponse> VerifyPaymentAsync(string transactionId, PaymentGatewayType? gatewayType = null)
        {
            // Auto-detect gateway if not provided
            var detectedGatewayType = gatewayType ?? DetectGatewayFromTransactionId(transactionId);

            return detectedGatewayType switch
            {
                PaymentGatewayType.Stripe => await VerifyStripePaymentAsync(transactionId),
                PaymentGatewayType.Razorpay => await VerifyRazorpayPaymentAsync(transactionId),
                _ => new PaymentVerificationResponse(false, PaymentStatus.Failed, null, $"Gateway {detectedGatewayType} is not supported")
            };
        }

        public async Task<RefundResponse> ProcessRefundAsync(RefundRequest request, PaymentGatewayType? gatewayType = null)
        {
            // Auto-detect gateway if not provided
            var detectedGatewayType = gatewayType ?? DetectGatewayFromTransactionId(request.OriginalTransactionId);

            return detectedGatewayType switch
            {
                PaymentGatewayType.Stripe => await ProcessStripeRefundAsync(request),
                PaymentGatewayType.Razorpay => await ProcessRazorpayRefundAsync(request),
                _ => new RefundResponse(false, null, $"Gateway {detectedGatewayType} is not supported")
            };
        }

        // Helper method to detect gateway from transaction ID patterns
        private PaymentGatewayType DetectGatewayFromTransactionId(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
            {
                _logger.LogWarning("Empty transaction ID provided");
                return PaymentGatewayType.Stripe; // Default fallback
            }

            // Stripe Payment Intent IDs start with "pi_"
            if (transactionId.StartsWith("pi_"))
            {
                return PaymentGatewayType.Stripe;
            }

            // Stripe Checkout Session IDs start with "cs_"
            if (transactionId.StartsWith("cs_"))
            {
                return PaymentGatewayType.Stripe;
            }

            // Razorpay Order IDs start with "order_"
            if (transactionId.StartsWith("order_"))
            {
                return PaymentGatewayType.Razorpay;
            }

            // Razorpay Payment IDs start with "pay_"
            if (transactionId.StartsWith("pay_"))
            {
                return PaymentGatewayType.Razorpay;
            }

            _logger.LogWarning("Unknown transaction ID format: {TransactionId}, defaulting to Stripe", transactionId);
            return PaymentGatewayType.Stripe; // Default fallback
        }

        // Helper methods
        public IEnumerable<PaymentGatewayType> GetAvailableGateways()
        {
            return new[] { PaymentGatewayType.Stripe, PaymentGatewayType.Razorpay };
        }

        public bool IsGatewayAvailable(PaymentGatewayType gatewayType)
        {
            return gatewayType == PaymentGatewayType.Stripe || gatewayType == PaymentGatewayType.Razorpay;
        }

        // Get gateway instance for direct access if needed
        public IPaymentGateway? GetGateway(PaymentGatewayType gatewayType)
        {
            return gatewayType switch
            {
                PaymentGatewayType.Stripe => _stripeGateway,
                PaymentGatewayType.Razorpay => _razorpayGateway,
                _ => null
            };
        }

        // Method to get publishable keys for frontend
        public string? GetPublishableKey(PaymentGatewayType gatewayType)
        {
            return gatewayType switch
            {
                PaymentGatewayType.Stripe => _stripeGateway.GetPublishableKey(),
                PaymentGatewayType.Razorpay => null, // Add if Razorpay has similar concept
                _ => null
            };
        }
    }
}