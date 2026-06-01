namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Địa chỉ giao hàng của người dùng. Mỗi user có thể có nhiều địa chỉ (1-n),
    /// trong đó tối đa một địa chỉ được đánh dấu mặc định.
    /// </summary>
    public class UserAddress : BaseEntity
    {
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;

        /// <summary>Nhãn gợi nhớ: Nhà riêng, Công ty...</summary>
        public string? Label { get; set; }

        public bool IsDefault { get; set; }
    }
}
