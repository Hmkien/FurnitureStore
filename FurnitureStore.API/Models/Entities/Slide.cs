namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Slide trong slider trang chủ.
    /// </summary>
    public class Slide : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string? LinkUrl { get; set; }
        public string? Caption { get; set; }
        public int SortOrder { get; set; }
    }
}
