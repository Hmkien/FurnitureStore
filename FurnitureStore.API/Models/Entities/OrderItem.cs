namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Dòng hàng trong đơn. Lưu snapshot tên/thuộc tính/giá tại thời điểm mua
    /// để đơn cũ không bị ảnh hưởng khi sản phẩm thay đổi sau này.
    /// </summary>
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public virtual Order? Order { get; set; }

        public Guid VariantId { get; set; }
        public virtual ProductVariant? Variant { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public string? VariantInfo { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }
}
