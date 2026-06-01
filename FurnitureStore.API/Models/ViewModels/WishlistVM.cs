namespace FurnitureStore.API.Models.ViewModels
{
    /// <summary>Một sản phẩm trong danh sách yêu thích của user.</summary>
    public class WishlistItemVM
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public decimal? MinPrice { get; set; }
        public DateTime Created { get; set; }
    }
}
