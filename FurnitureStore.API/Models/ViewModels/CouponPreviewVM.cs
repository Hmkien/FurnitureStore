using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.ViewModels
{
    public class CouponPreviewVM
    {
        public string Code { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal AmountAfterDiscount { get; set; }
    }

    /// <summary>Mã giảm giá đang hoạt động, công khai cho storefront.</summary>
    public class CouponPublicVM
    {
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal MaxDiscount { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
