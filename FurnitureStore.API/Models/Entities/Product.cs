namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Sản phẩm chung. Giá/tồn kho nằm ở từng <see cref="ProductVariant"/>.
    /// </summary>
    public class Product : BaseEntity
    {
        public Guid CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }

        /// <summary>Phong cách: Hiện đại, Cổ điển, Tối giản...</summary>
        public string? Style { get; set; }

        public virtual ICollection<ProductVariant>? Variants { get; set; }
        public virtual ICollection<ProductImage>? Images { get; set; }
    }
}
