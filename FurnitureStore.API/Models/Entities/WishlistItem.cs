namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Sản phẩm yêu thích của một người dùng (mỗi user - mỗi sản phẩm tối đa một dòng).
    /// </summary>
    public class WishlistItem : BaseEntity
    {
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        public Guid ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}
