using FurnitureStore.API.Models.ViewModels;

namespace FurnitureStore.API.Interface
{
    /// <summary>Báo cáo & thống kê (dành cho quản trị).</summary>
    public interface IReportService
    {
        Task<RevenueSummaryVM> GetRevenueSummaryAsync(DateTime? from, DateTime? to);
        Task<List<RevenuePointVM>> GetRevenueByDayAsync(DateTime from, DateTime to);
        Task<List<TopProductVM>> GetTopProductsAsync(DateTime? from, DateTime? to, int top);
        Task<List<OrderStatusCountVM>> GetOrderStatusBreakdownAsync();
        Task<List<LowStockVariantVM>> GetLowStockAsync(int threshold);
    }
}
