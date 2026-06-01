using System.ComponentModel.DataAnnotations;
using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.Form
{
    /// <summary>
    /// Thông tin đặt hàng từ giỏ hàng hiện tại của user.
    /// </summary>
    public class CheckoutForm
    {
        [Required(ErrorMessage = "Tên người nhận là bắt buộc")]
        [StringLength(150)]
        public string ReceiverName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(20)]
        public string ReceiverPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc")]
        [StringLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Note { get; set; }

        [StringLength(50)]
        public string? CouponCode { get; set; }

        // Phí vận chuyển KHÔNG nhận từ client — được tính server-side trong OrderService.

        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
        public PaymentMethod PaymentMethod { get; set; }
    }

    public class UpdateOrderStatusForm
    {
        [Required]
        public OrderStatus OrderStatus { get; set; }
    }

    public class UpdatePaymentStatusForm
    {
        [Required]
        public PaymentStatus PaymentStatus { get; set; }
    }
}
