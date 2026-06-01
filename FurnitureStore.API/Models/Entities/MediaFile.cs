namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// File trong kho media (ảnh sản phẩm/banner/slide/bài viết...).
    /// Lưu metadata; file vật lý nằm trong wwwroot.
    /// </summary>
    public class MediaFile : BaseEntity
    {
        public string FileName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public long Size { get; set; }
        public string? Folder { get; set; }
    }
}
