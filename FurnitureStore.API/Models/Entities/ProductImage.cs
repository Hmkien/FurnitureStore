namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Ảnh của sản phẩm. <see cref="IsPrimary"/> đánh dấu ảnh đại diện.
    /// </summary>
    public class ProductImage : BaseEntity
    {
        public Guid ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}
