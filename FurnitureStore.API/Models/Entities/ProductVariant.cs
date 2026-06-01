using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Biến thể sản phẩm (size/chất liệu/màu) — đơn vị có giá và tồn kho riêng.
    /// </summary>
    public class ProductVariant : BaseEntity
    {
        public Guid ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public string? Size { get; set; }
        public string? Material { get; set; }
        public string? Color { get; set; }

        /// <summary>Tình trạng hàng: Mới, Like-new 99%, hàng trả lại...</summary>
        public ProductCondition Condition { get; set; } = ProductCondition.New;

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string SkuVariant { get; set; } = string.Empty;
    }
}
