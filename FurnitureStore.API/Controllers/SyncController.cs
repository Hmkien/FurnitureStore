using FurnitureStore.API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly ISyncService _syncService;

        public SyncController(ISyncService syncService)
        {
            _syncService = syncService;
        }

        /// <summary>Cho biết tính năng giả lập có đang bật không (để ẩn/hiện nút ở UI).</summary>
        [AllowAnonymous]
        [HttpGet("enabled")]
        public IActionResult IsEnabled() => Ok(new { enabled = _syncService.Enabled });

        /// <summary>Seed sản phẩm thật từ moho.com.vn. Mỗi lần gọi tạo ra các sản phẩm KHÁC NHAU (mặc định 5).</summary>
        [HttpPost("moho")]
        public async Task<IActionResult> SeedMoho([FromQuery] int count = 5)
        {
            if (!_syncService.Enabled)
                return NotFound(new { error = "Tính năng giả lập đang tắt" });

            var created = await _syncService.SeedMohoProductsAsync(count);
            return Ok(new
            {
                created,
                message = created == 0
                    ? "Đã seed hết sản phẩm Moho có sẵn"
                    : $"Đã thêm {created} sản phẩm từ moho.com.vn"
            });
        }

        /// <summary>Tạo đơn hàng giả lập với nhiều trạng thái khác nhau (mặc định 10).</summary>
        [HttpPost("orders")]
        public async Task<IActionResult> SeedOrders([FromQuery] int count = 10)
        {
            if (!_syncService.Enabled)
                return NotFound(new { error = "Tính năng giả lập đang tắt" });

            var created = await _syncService.GenerateOrdersAsync(count);
            return Ok(new { created, message = $"Đã tạo {created} đơn hàng giả lập" });
        }

        /// <summary>Tạo dữ liệu mẫu ngẫu nhiên cho một resource (category, product, post, banner, slide, coupon, membershiptier).</summary>
        [HttpPost("{resource}")]
        public async Task<IActionResult> Generate(string resource, [FromQuery] int count = 10)
        {
            if (!_syncService.Enabled)
                return NotFound(new { error = "Tính năng giả lập đang tắt" });

            var created = await _syncService.GenerateAsync(resource, count);
            return Ok(new { created, message = $"Đã tạo {created} bản ghi mẫu" });
        }
    }
}
