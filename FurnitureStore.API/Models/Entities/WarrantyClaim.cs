using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Phiếu yêu cầu bảo hành cho một sản phẩm đã mua.
    /// </summary>
    public class WarrantyClaim : BaseEntity
    {
        public string Code { get; set; } = string.Empty;

        public Guid OrderId { get; set; }
        public virtual Order? Order { get; set; }

        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        public Guid VariantId { get; set; }
        public virtual ProductVariant? Variant { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string IssueDescription { get; set; } = string.Empty;

        public WarrantyStatus ClaimStatus { get; set; } = WarrantyStatus.Received;
        public string? Note { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
