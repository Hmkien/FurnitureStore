using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        /// <summary>Nhập / xuất / điều chỉnh tồn kho cho một biến thể.</summary>
        [HttpPost("adjust")]
        public async Task<IActionResult> Adjust([FromBody] StockAdjustForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var variant = await _inventoryService.AdjustAsync(form);
            return Ok(new { message = "Cập nhật kho thành công", variant.Id, variant.StockQuantity });
        }

        /// <summary>Lịch sử biến động kho của một biến thể.</summary>
        [HttpPost("history/{variantId}")]
        public async Task<IActionResult> History(Guid variantId, [FromBody] BaseQuery query)
            => Ok(await _inventoryService.GetHistoryAsync(variantId, query));

        /// <summary>Cảnh báo hàng sắp hết.</summary>
        [HttpGet("low-stock")]
        public async Task<IActionResult> LowStock([FromQuery] int threshold = 5)
            => Ok(await _inventoryService.GetLowStockAsync(threshold));
    }
}
