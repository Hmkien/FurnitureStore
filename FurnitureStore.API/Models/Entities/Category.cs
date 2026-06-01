 using System.Text.Json.Serialization;

namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Danh mục sản phẩm, hỗ trợ đa cấp qua <see cref="ParentId"/>.
    /// </summary>
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public Guid? ParentId { get; set; }
        public virtual Category? Parent { get; set; }
        [JsonIgnore]
        public virtual ICollection<Category>? Children { get; set; }

        [JsonIgnore]
        public virtual ICollection<Product>? Products { get; set; }
    }
}
