namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Banner quảng cáo/khuyến mãi hiển thị theo vị trí trên website.
    /// </summary>
    public class Banner : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string? LinkUrl { get; set; }
        /// <summary>Vị trí hiển thị: home_top, sidebar, category...</summary>
        public string? Position { get; set; }
        public int SortOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
