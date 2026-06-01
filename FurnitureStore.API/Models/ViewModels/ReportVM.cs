using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.ViewModels
{
    public class RevenueSummaryVM
    {
        public decimal TotalRevenue { get; set; }
        public int CompletedOrders { get; set; }
        public int ItemsSold { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class RevenuePointVM
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }

    public class TopProductVM
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class OrderStatusCountVM
    {
        public OrderStatus OrderStatus { get; set; }
        public int Count { get; set; }
    }

    public class LowStockVariantVM
    {
        public Guid VariantId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SkuVariant { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }
}
