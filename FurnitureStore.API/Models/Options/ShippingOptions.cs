namespace FurnitureStore.API.Models.Options
{
    /// <summary>
    /// Cấu hình phí vận chuyển (section "Shipping" trong appsettings).
    /// Phí được tính server-side, không tin tưởng giá trị do client gửi.
    /// </summary>
    public class ShippingOptions
    {
        /// <summary>Phí vận chuyển cố định mỗi đơn (VND).</summary>
        public decimal FlatFee { get; set; } = 50000m;

        /// <summary>Miễn phí vận chuyển khi tạm tính >= ngưỡng này (VND). 0 = không áp dụng.</summary>
        public decimal FreeShippingThreshold { get; set; } = 0m;
    }
}
