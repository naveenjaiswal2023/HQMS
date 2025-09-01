using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Commands;
using PaymentService.Application.Exceptions;
using PaymentService.Domain.Enums;
using PaymentService.Domain.Models.Payments;
using PaymentService.Infrastructure.PaymentGateways;
using System.Text.Json;

namespace PaymentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly PaymentGatewayManager _paymentManager;

        public PaymentsController(IMediator mediator, PaymentGatewayManager paymentManager)
        {
            _mediator = mediator;
            _paymentManager = paymentManager;
        }

        [HttpPost("initiate-registration")]
        public async Task<ActionResult<InitiatePaymentResult>> InitiateRegistrationPayment([FromBody] InitiateRegistrationPaymentCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("verify/{transactionId}")]
        public async Task<ActionResult<PaymentVerificationResult>> VerifyPayment(string transactionId)
        {
            try
            {
                var command = new VerifyPaymentCommand(transactionId);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] object webhookData)
        {
            try
            {
                // Process Razorpay webhook
                var json = webhookData.ToString();
                //var webhookEvent = JsonSerializer.Deserialize<RazorpayWebhookEvent>(json);

                //if (webhookEvent.Event == "payment.captured")
                //{
                //    var command = new VerifyPaymentCommand(webhookEvent.Payload.Order.Receipt);
                //    await _mediator.Send(command);
                //}

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost("stripe/initiate")]
        public async Task<IActionResult> InitiateStripePayment([FromBody] PaymentInitiationRequest request)
        {
            var result = await _paymentManager.InitiateStripePaymentAsync(request);
            return Ok(result);
        }

        [HttpPost("razorpay/initiate")]
        public async Task<IActionResult> InitiateRazorpayPayment([FromBody] PaymentInitiationRequest request)
        {
            var result = await _paymentManager.InitiateRazorpayPaymentAsync(request);
            return Ok(result);
        }

        // Generic method with query parameter
        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment(
            [FromBody] PaymentInitiationRequest request,
            [FromQuery] string gateway = "stripe")
        {
            var gatewayType = gateway.ToLower() switch
            {
                "stripe" => PaymentGatewayType.Stripe,
                "razorpay" => PaymentGatewayType.Razorpay,
                _ => PaymentGatewayType.Stripe
            };

            var result = await _paymentManager.InitiatePaymentAsync(request, gatewayType);
            return Ok(result);
        }

        //[HttpPost("verify/{transactionId}")]
        //public async Task<IActionResult> VerifyPaymentByGateway(string transactionId)
        //{
        //    var result = await _paymentManager.VerifyPaymentAsync(transactionId);
        //    return Ok(result);
        //}

        // Explicit gateway verification
        [HttpPost("stripe/verify/{transactionId}")]
        public async Task<IActionResult> VerifyStripePayment(string transactionId)
        {
            var result = await _paymentManager.VerifyStripePaymentAsync(transactionId);
            return Ok(result);
        }

        [HttpPost("razorpay/verify/{transactionId}")]
        public async Task<IActionResult> VerifyRazorpayPayment(string transactionId)
        {
            var result = await _paymentManager.VerifyRazorpayPaymentAsync(transactionId);
            return Ok(result);
        }

        [HttpPost("refund")]
        public async Task<IActionResult> ProcessRefund([FromBody] RefundRequest request)
        {
            var result = await _paymentManager.ProcessRefundAsync(request);
            return Ok(result);
        }

        // Get available gateways
        [HttpGet("gateways")]
        public IActionResult GetAvailableGateways()
        {
            var gateways = _paymentManager.GetAvailableGateways();
            return Ok(gateways);
        }

        // Get publishable key for frontend
        [HttpGet("config/{gateway}")]
        public IActionResult GetGatewayConfig(string gateway)
        {
            if (Enum.TryParse<PaymentGatewayType>(gateway, true, out var gatewayType))
            {
                var key = _paymentManager.GetPublishableKey(gatewayType);
                return Ok(new { Gateway = gateway, PublishableKey = key });
            }

            return BadRequest("Invalid gateway");
        }
    }
}
