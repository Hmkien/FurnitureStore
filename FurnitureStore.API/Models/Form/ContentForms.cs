using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.API.Models.Form
{
    public class PostForm
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(250)]
        public string Title { get; set; } = string.Empty;

        [StringLength(280)]
        public string? Slug { get; set; }

        [StringLength(500)]
        public string? Summary { get; set; }

        public string? Content { get; set; }

        [StringLength(500)]
        public string? ThumbnailUrl { get; set; }

        [StringLength(120)]
        public string? Category { get; set; }

        [StringLength(150)]
        public string? Author { get; set; }

        public bool IsPublished { get; set; }
    }

    public class BannerForm
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ảnh là bắt buộc")]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(500)]
        public string? LinkUrl { get; set; }

        [StringLength(100)]
        public string? Position { get; set; }

        public int SortOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class SlideForm
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ảnh là bắt buộc")]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(500)]
        public string? LinkUrl { get; set; }

        [StringLength(300)]
        public string? Caption { get; set; }

        public int SortOrder { get; set; }
    }
}
