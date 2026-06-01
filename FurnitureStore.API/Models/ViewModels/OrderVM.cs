using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.ViewModels
{
    /// <summary>Dòng đơn hàng cho danh sách.</summary>
    public class OrderVM
    {
        public Guid Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? CouponCode { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int ItemCount { get; set; }
        public DateTime Created { get; set; }

        /// <summary>Tên sản phẩm đầu tiên trong đơn (hiển thị nhanh ở danh sách).</summary>
        public string? FirstProductName { get; set; }
        /// <summary>Ảnh sản phẩm đầu tiên trong đơn.</summary>
        public string? ThumbnailUrl { get; set; }
    }

    public class OrderDetailVM : OrderVM
    {
        public Guid UserId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Note { get; set; }
        public List<OrderItemVM> Items { get; set; } = new();
    }

    public class OrderItemVM
    {
        public Guid Id { get; set; }
        public Guid VariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? VariantInfo { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }
}
