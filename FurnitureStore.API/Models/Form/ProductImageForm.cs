using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.API.Models.Form
{
    public class ProductImageForm
    {
        [Required(ErrorMessage = "Sản phẩm là bắt buộc")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Đường dẫn ảnh là bắt buộc")]
        [StringLength(500, ErrorMessage = "Đường dẫn ảnh tối đa 500 ký tự")]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }
    }
}
