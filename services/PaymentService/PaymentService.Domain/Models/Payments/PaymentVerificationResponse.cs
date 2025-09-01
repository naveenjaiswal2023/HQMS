using PaymentService.Domain.Enums;

namespace PaymentService.Domain.Models.Payments
{
    public class PaymentVerificationResponse
    {
        public bool IsSuccess { get; set; }
        public PaymentStatus Status { get; set; }
        public string? GatewayResponse { get; set; }
        public string? ErrorMessage { get; set; }

        public PaymentVerificationResponse(
            bool isSuccess,
            PaymentStatus status,
            string? gatewayResponse,
            string? errorMessage)
        {
            IsSuccess = isSuccess;
            Status = status;
            GatewayResponse = gatewayResponse;
            ErrorMessage = errorMessage;
        }
    }
}
