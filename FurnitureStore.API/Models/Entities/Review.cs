namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Đánh giá sản phẩm của khách đã mua (Rating 1-5).
    /// Dùng <see cref="BaseEntity.Status"/> để duyệt/ẩn đánh giá.
    /// </summary>
    public class Review : BaseEntity
    {
        public Guid ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? ImageUrl { get; set; }
    }
}
