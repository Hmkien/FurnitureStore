using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Yêu cầu đổi/trả/hoàn tiền của khách cho một đơn hàng.
    /// </summary>
    public class ReturnRequest : BaseEntity
    {
        public string Code { get; set; } = string.Empty;

        public Guid OrderId { get; set; }
        public virtual Order? Order { get; set; }

        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        public ReturnType Type { get; set; }
        public ReturnStatus RequestStatus { get; set; } = ReturnStatus.Requested;
        public string Reason { get; set; } = string.Empty;
        public string? Note { get; set; }

        public virtual ICollection<ReturnItem>? Items { get; set; }
    }

    /// <summary>Một dòng sản phẩm trong yêu cầu đổi/trả (snapshot tên + giá).</summary>
    public class ReturnItem : BaseEntity
    {
        public Guid ReturnRequestId { get; set; }
        public virtual ReturnRequest? ReturnRequest { get; set; }

        public Guid VariantId { get; set; }
        public virtual ProductVariant? Variant { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
