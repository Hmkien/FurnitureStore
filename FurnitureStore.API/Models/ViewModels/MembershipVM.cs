namespace FurnitureStore.API.Models.ViewModels
{
    public class MyMembershipVM
    {
        public decimal TotalSpending { get; set; }
        public string? CurrentTier { get; set; }
        public decimal CurrentDiscountPercent { get; set; }
        public string? NextTier { get; set; }
        public decimal? AmountToNextTier { get; set; }
    }
}
