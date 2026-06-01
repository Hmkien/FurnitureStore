using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Lịch sử biến động tồn kho của một biến thể (nhập/xuất/điều chỉnh).
    /// </summary>
    public class StockMovement : BaseEntity
    {
        public Guid VariantId { get; set; }
        public virtual ProductVariant? Variant { get; set; }

        public StockMovementType Type { get; set; }
        /// <summary>Số lượng thay đổi (luôn dương; ý nghĩa tăng/giảm theo Type).</summary>
        public int Quantity { get; set; }
        /// <summary>Tồn kho sau khi áp dụng biến động.</summary>
        public int QuantityAfter { get; set; }
        public string? Note { get; set; }
    }
}
