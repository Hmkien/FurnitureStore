using FurnitureStore.API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("revenue-summary")]
        public async Task<IActionResult> RevenueSummary([FromQuery] DateTime? from, [FromQuery] DateTime? to)
            => Ok(await _reportService.GetRevenueSummaryAsync(from, to));

        [HttpGet("revenue-by-day")]
        public async Task<IActionResult> RevenueByDay([FromQuery] DateTime from, [FromQuery] DateTime to)
            => Ok(await _reportService.GetRevenueByDayAsync(from, to));

        [HttpGet("top-products")]
        public async Task<IActionResult> TopProducts([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int top = 10)
            => Ok(await _reportService.GetTopProductsAsync(from, to, top));

        [HttpGet("order-status-breakdown")]
        public async Task<IActionResult> OrderStatusBreakdown()
            => Ok(await _reportService.GetOrderStatusBreakdownAsync());

        [HttpGet("low-stock")]
        public async Task<IActionResult> LowStock([FromQuery] int threshold = 5)
            => Ok(await _reportService.GetLowStockAsync(threshold));
    }
}
