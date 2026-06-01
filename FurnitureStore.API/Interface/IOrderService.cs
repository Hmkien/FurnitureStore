using FurnitureStore.API.Models.Enums;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.Query;
using FurnitureStore.API.Models.ViewModels;

namespace FurnitureStore.API.Interface
{
    public interface IOrderService
    {
        /// <summary>Đặt hàng từ giỏ hàng hiện tại của user (trừ tồn kho, snapshot giá).</summary>
        Task<OrderDetailVM> CheckoutAsync(CheckoutForm form);

        /// <summary>Báo phí vận chuyển cho một mức tạm tính (chỉ để hiển thị; phí thật tính khi checkout).</summary>
        ShippingQuoteVM QuoteShipping(decimal subTotal);

        /// <summary>Đơn hàng của user đang đăng nhập (phân trang, lọc theo trạng thái).</summary>
        Task<DataTableJson> GetMyOrdersAsync(OrderQuery query);

        /// <summary>Toàn bộ đơn hàng (dành cho quản trị).</summary>
        Task<DataTableJson> GetAllAsync(BaseQuery query);

        /// <summary>Chi tiết đơn (chủ đơn hoặc quản trị).</summary>
        Task<OrderDetailVM> GetDetailAsync(Guid id);

        /// <summary>Cập nhật trạng thái xử lý đơn (quản trị).</summary>
        Task UpdateStatusAsync(Guid id, OrderStatus status);

        /// <summary>Cập nhật trạng thái thanh toán (quản trị).</summary>
        Task UpdatePaymentStatusAsync(Guid id, PaymentStatus status);

        /// <summary>Hủy đơn (chủ đơn khi đơn chưa giao) — hoàn lại tồn kho.</summary>
        Task CancelAsync(Guid id);
    }
}
