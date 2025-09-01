namespace PaymentService.Domain.Models.Payments
{
    public class RefundResponse
    {
        public bool IsSuccess { get; set; }
        public string? RefundId { get; set; }
        public string? ErrorMessage { get; set; }

        public RefundResponse(bool isSuccess, string? refundId, string? errorMessage)
        {
            IsSuccess = isSuccess;
            RefundId = refundId;
            ErrorMessage = errorMessage;
        }
    }
}
