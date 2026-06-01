using FurnitureStore.API.Data;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Middlewares;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Enums;
using FurnitureStore.API.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly RequestContext _requestContext;

        public ReportService(ApplicationDbContext context, RequestContext requestContext)
        {
            _context = context;
            _requestContext = requestContext;
        }

        public async Task<RevenueSummaryVM> GetRevenueSummaryAsync(DateTime? from, DateTime? to)
        {
            EnsureManager();
            var orders = CompletedOrders(from, to);

            var totalRevenue = await orders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
            var completedOrders = await orders.CountAsync();
            var itemsSold = await orders.SelectMany(o => o.Items!).SumAsync(i => (int?)i.Quantity) ?? 0;

            return new RevenueSummaryVM
            {
                TotalRevenue = totalRevenue,
                CompletedOrders = completedOrders,
                ItemsSold = itemsSold,
                AverageOrderValue = completedOrders > 0 ? decimal.Round(totalRevenue / completedOrders, 2) : 0
            };
        }

        public async Task<List<RevenuePointVM>> GetRevenueByDayAsync(DateTime from, DateTime to)
        {
            EnsureManager();
            return await CompletedOrders(from, to)
                .GroupBy(o => o.Created.Date)
                .Select(g => new RevenuePointVM
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount),
                    Orders = g.Count()
                })
                .OrderBy(p => p.Date)
                .ToListAsync();
        }

        public async Task<List<TopProductVM>> GetTopProductsAsync(DateTime? from, DateTime? to, int top)
        {
            EnsureManager();
            if (top <= 0) top = 10;

            var items = _context.OrderItems
                .Where(oi => oi.Order!.OrderStatus == OrderStatus.Completed);

            if (from.HasValue) items = items.Where(oi => oi.Order!.Created >= from.Value);
            if (to.HasValue) items = items.Where(oi => oi.Order!.Created <= to.Value);

            return await items
                .GroupBy(oi => new { oi.Variant!.ProductId, oi.ProductName })
                .Select(g => new TopProductVM
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    QuantitySold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.LineTotal)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(top)
                .ToListAsync();
        }

        public async Task<List<OrderStatusCountVM>> GetOrderStatusBreakdownAsync()
        {
            EnsureManager();
            return await _context.Orders
                .GroupBy(o => o.OrderStatus)
                .Select(g => new OrderStatusCountVM
                {
                    OrderStatus = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
        }

        public async Task<List<LowStockVariantVM>> GetLowStockAsync(int threshold)
        {
            EnsureManager();
            if (threshold < 0) threshold = 0;

            return await _context.ProductVariants
                .Where(v => v.StockQuantity <= threshold)
                .OrderBy(v => v.StockQuantity)
                .Select(v => new LowStockVariantVM
                {
                    VariantId = v.Id,
                    ProductId = v.ProductId,
                    ProductName = v.Product!.Name,
                    SkuVariant = v.SkuVariant,
                    StockQuantity = v.StockQuantity
                })
                .ToListAsync();
        }

        private IQueryable<Order> CompletedOrders(DateTime? from, DateTime? to)
        {
            var query = _context.Orders.Where(o => o.OrderStatus == OrderStatus.Completed);
            if (from.HasValue) query = query.Where(o => o.Created >= from.Value);
            if (to.HasValue) query = query.Where(o => o.Created <= to.Value);
            return query;
        }

        private void EnsureManager()
        {
            if (!_requestContext.IsSuperUser() && !_requestContext.HasPermission("REPORT_VIEW"))
                throw new UnauthorizedAccessException("Bạn không có quyền xem báo cáo");
        }
    }
}
