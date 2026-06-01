namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Tin bài / bài viết (tin tức, xu hướng thiết kế nội thất).
    /// </summary>
    public class Post : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string? Content { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Category { get; set; }
        public string? Author { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
}
