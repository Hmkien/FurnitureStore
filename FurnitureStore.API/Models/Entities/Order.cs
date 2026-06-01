using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Đơn hàng. Tổng tiền = tổng dòng hàng + phí vận chuyển.
    /// </summary>
    public class Order : BaseEntity
    {
        public string OrderCode { get; set; } = string.Empty;

        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? CouponCode { get; set; }
        public decimal TotalAmount { get; set; }

        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Note { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        public virtual ICollection<OrderItem>? Items { get; set; }
    }
}
