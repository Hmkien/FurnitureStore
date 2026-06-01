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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>Đặt hàng từ giỏ hàng hiện tại.</summary>
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _orderService.CheckoutAsync(form);
            return StatusCode(StatusCodes.Status201Created, order);
        }

        /// <summary>Báo phí vận chuyển theo tạm tính (hiển thị ở trang thanh toán/giỏ).</summary>
        [AllowAnonymous]
        [HttpGet("shipping-quote")]
        public IActionResult ShippingQuote([FromQuery] decimal subTotal)
            => Ok(_orderService.QuoteShipping(subTotal));

        /// <summary>Đơn hàng của tôi (lọc theo trạng thái + tìm kiếm).</summary>
        [HttpPost("my/GetPaged")]
        public async Task<IActionResult> GetMyOrders([FromBody] OrderQuery query)
            => Ok(await _orderService.GetMyOrdersAsync(query));

        /// <summary>Toàn bộ đơn hàng (quản trị).</summary>
        [HttpPost("GetPaged")]
        public async Task<IActionResult> GetAll([FromBody] BaseQuery query)
            => Ok(await _orderService.GetAllAsync(query));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(Guid id)
            => Ok(await _orderService.GetDetailAsync(id));

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _orderService.UpdateStatusAsync(id, form.OrderStatus);
            return NoContent();
        }

        [HttpPut("{id}/payment-status")]
        public async Task<IActionResult> UpdatePaymentStatus(Guid id, [FromBody] UpdatePaymentStatusForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _orderService.UpdatePaymentStatusAsync(id, form.PaymentStatus);
            return NoContent();
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            await _orderService.CancelAsync(id);
            return NoContent();
        }
    }
}
