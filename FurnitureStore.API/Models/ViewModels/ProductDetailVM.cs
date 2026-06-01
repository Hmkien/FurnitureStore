using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.ViewModels
{
    /// <summary>Dòng sản phẩm cho danh sách quản trị (kèm ảnh đại diện).</summary>
    public class ProductListVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string? Style { get; set; }
        public Guid CategoryId { get; set; }
        public string? ThumbnailUrl { get; set; }
        public decimal? MinPrice { get; set; }
        public StatusEntity Status { get; set; }
        public DateTime Created { get; set; }
    }

    /// <summary>
    /// Chi tiết sản phẩm cho storefront: gồm danh mục, biến thể và thư viện ảnh.
    /// </summary>
    public class ProductDetailVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public string? Style { get; set; }

        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public List<ProductVariantVM> Variants { get; set; } = new();
        public List<ProductImageVM> Images { get; set; } = new();
    }

    public class ProductVariantVM
    {
        public Guid Id { get; set; }
        public string? Size { get; set; }
        public string? Material { get; set; }
        public string? Color { get; set; }
        public Enums.ProductCondition Condition { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string SkuVariant { get; set; } = string.Empty;
    }

    public class ProductImageVM
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}
