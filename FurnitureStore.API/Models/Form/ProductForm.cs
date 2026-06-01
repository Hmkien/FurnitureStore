using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.API.Models.Form
{
    public class ProductForm
    {
        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(250, ErrorMessage = "Tên sản phẩm tối đa 250 ký tự")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Để trống sẽ tự sinh từ Name.</summary>
        [StringLength(280)]
        public string? Slug { get; set; }

        [Required(ErrorMessage = "SKU là bắt buộc")]
        [StringLength(100, ErrorMessage = "SKU tối đa 100 ký tự")]
        public string Sku { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả ngắn tối đa 500 ký tự")]
        public string? ShortDescription { get; set; }

        public string? LongDescription { get; set; }

        [StringLength(100)]
        public string? Style { get; set; }
    }
}
