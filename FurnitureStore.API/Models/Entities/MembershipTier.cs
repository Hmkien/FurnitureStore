namespace FurnitureStore.API.Models.Entities
{
    /// <summary>
    /// Hạng thành viên (Đồng, Bạc, Vàng, Kim cương) xác định theo tổng chi tiêu.
    /// </summary>
    public class MembershipTier : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        /// <summary>Tổng chi tiêu tối thiểu để đạt hạng.</summary>
        public decimal MinSpending { get; set; }
        /// <summary>% chiết khấu đặc quyền của hạng.</summary>
        public decimal DiscountPercent { get; set; }
        public string? Description { get; set; }
    }
}
