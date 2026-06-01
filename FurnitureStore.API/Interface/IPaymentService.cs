using FurnitureStore.API.Models.Payment;

namespace FurnitureStore.API.Interface
{
    public interface IPaymentService
    {
        /// <summary>Tạo URL thanh toán VNPAY cho một đơn (theo OrderCode).</summary>
        Task<CreatePaymentResultVM> CreateVnpayUrlAsync(string orderCode, string ipAddress);

        /// <summary>Xử lý callback VNPAY: xác thực chữ ký và cập nhật trạng thái thanh toán.</summary>
        Task<PaymentResultVM> HandleVnpayReturnAsync(IQueryCollection query);

        /// <summary>Tạo URL thanh toán Momo cho một đơn (theo OrderCode).</summary>
        Task<CreatePaymentResultVM> CreateMomoUrlAsync(string orderCode);

        /// <summary>Xử lý callback Momo: kiểm tra kết quả và cập nhật trạng thái thanh toán.</summary>
        Task<PaymentResultVM> HandleMomoReturnAsync(IQueryCollection query);
    }
}
