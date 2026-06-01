using System.ComponentModel;

namespace FurnitureStore.API.Models.Enums
{
    /// <summary>Phương thức thanh toán.</summary>
    public enum PaymentMethod
    {
        [Description("Thanh toán khi nhận hàng")]
        COD = 1,
        [Description("Chuyển khoản ngân hàng")]
        BankTransfer = 2,
        [Description("VNPAY")]
        VNPAY = 3,
        [Description("Momo")]
        Momo = 4
    }

    /// <summary>Trạng thái thanh toán.</summary>
    public enum PaymentStatus
    {
        [Description("Chưa thanh toán")]
        Unpaid = 1,
        [Description("Đã thanh toán")]
        Paid = 2,
        [Description("Đã hoàn tiền")]
        Refunded = 3
    }

    /// <summary>Vòng đời xử lý đơn hàng.</summary>
    public enum OrderStatus
    {
        [Description("Chờ xác nhận")]
        Pending = 1,
        [Description("Đã xác nhận")]
        Confirmed = 2,
        [Description("Đang chuẩn bị hàng")]
        Preparing = 3,
        [Description("Đang giao hàng")]
        Shipping = 4,
        [Description("Đã giao / Hoàn thành")]
        Completed = 5,
        [Description("Đã hủy")]
        Cancelled = 6
    }
}
