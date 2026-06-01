namespace FurnitureStore.API.Models.ViewModels
{
    /// <summary>Báo phí vận chuyển để hiển thị trước khi đặt hàng (chỉ để hiển thị).</summary>
    public class ShippingQuoteVM
    {
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal FreeShippingThreshold { get; set; }
        public bool IsFreeShipping { get; set; }
    }
}
