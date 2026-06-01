using System.ComponentModel.DataAnnotations;
using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.Form
{
    public class ProductVariantForm
    {
        [Required(ErrorMessage = "Sản phẩm là bắt buộc")]
        public Guid ProductId { get; set; }

        [StringLength(100)]
        public string? Size { get; set; }

        [StringLength(100)]
        public string? Material { get; set; }

        [StringLength(100)]
        public string? Color { get; set; }

        public ProductCondition Condition { get; set; } = ProductCondition.New;

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải >= 0")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Tồn kho phải >= 0")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "SKU biến thể là bắt buộc")]
        [StringLength(120, ErrorMessage = "SKU biến thể tối đa 120 ký tự")]
        public string SkuVariant { get; set; } = string.Empty;
    }
}
