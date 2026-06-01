namespace FurnitureStore.API.Models.Payment
{
    /// <summary>Kết quả xử lý callback thanh toán.</summary>
    public class PaymentResultVM
    {
        public bool Success { get; set; }
        public string? OrderCode { get; set; }
        public string? TransactionId { get; set; }
        public string? ResponseCode { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>Phản hồi tạo yêu cầu thanh toán (trả URL để redirect).</summary>
    public class CreatePaymentResultVM
    {
        public string PaymentUrl { get; set; } = string.Empty;
    }
}
