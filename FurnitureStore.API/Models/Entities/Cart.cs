namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Giỏ hàng. Thuộc về một user đã đăng nhập (<see cref="UserId"/>) HOẶC một
    /// khách ẩn danh được nhận diện qua <see cref="GuestToken"/> (header X-Cart-Token).
    /// Khi khách đăng nhập, giỏ guest được merge vào giỏ user.
    /// </summary>
    public class Cart : BaseEntity
    {
        public Guid? UserId { get; set; }
        public virtual User? User { get; set; }

        /// <summary>Định danh giỏ của khách chưa đăng nhập (null khi đã có UserId).</summary>
        public string? GuestToken { get; set; }

        public virtual ICollection<CartItem>? Items { get; set; }
    }
}
