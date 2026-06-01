namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Một dòng trong giỏ hàng, trỏ tới biến thể sản phẩm cụ thể.
    /// </summary>
    public class CartItem : BaseEntity
    {
        public Guid CartId { get; set; }
        public virtual Cart? Cart { get; set; }

        public Guid VariantId { get; set; }
        public virtual ProductVariant? Variant { get; set; }

        public int Quantity { get; set; }
    }
}
