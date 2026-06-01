using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Mã giảm giá áp dụng khi đặt hàng.
    /// </summary>
    public class Coupon : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }

        public DiscountType DiscountType { get; set; }
        /// <summary>% (0-100) nếu Percentage, hoặc số tiền nếu Fixed.</summary>
        public decimal DiscountValue { get; set; }

        /// <summary>Giá trị đơn tối thiểu để áp dụng.</summary>
        public decimal MinOrderAmount { get; set; }
        /// <summary>Mức giảm tối đa (cho loại %). 0 = không giới hạn.</summary>
        public decimal MaxDiscount { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        /// <summary>Số lần được dùng tối đa. 0 = không giới hạn.</summary>
        public int UsageLimit { get; set; }
        public int UsedCount { get; set; }
    }
}
