using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.API.Models.Form
{
    public class CategoryForm
    {
        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên danh mục tối đa 200 ký tự")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Để trống sẽ tự sinh từ Name.</summary>
        [StringLength(220)]
        public string? Slug { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
        public string? Description { get; set; }

        [StringLength(500, ErrorMessage = "Đường dẫn ảnh tối đa 500 ký tự")]
        public string? ImageUrl { get; set; }

        public Guid? ParentId { get; set; }
    }
}
