using FurnitureStore.API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>Tạo URL thanh toán VNPAY cho đơn (frontend redirect tới URL trả về).</summary>
        [HttpGet("vnpay/create/{orderCode}")]
        public async Task<IActionResult> CreateVnpay(string orderCode)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            return Ok(await _paymentService.CreateVnpayUrlAsync(orderCode, ip));
        }

        /// <summary>Callback VNPAY (gọi bởi cổng thanh toán, không cần đăng nhập).</summary>
        [AllowAnonymous]
        [HttpGet("vnpay/return")]
        public async Task<IActionResult> VnpayReturn()
            => Ok(await _paymentService.HandleVnpayReturnAsync(Request.Query));

        /// <summary>Tạo URL thanh toán Momo cho đơn.</summary>
        [HttpGet("momo/create/{orderCode}")]
        public async Task<IActionResult> CreateMomo(string orderCode)
            => Ok(await _paymentService.CreateMomoUrlAsync(orderCode));

        /// <summary>Callback Momo (return/notify).</summary>
        [AllowAnonymous]
        [HttpGet("momo/return")]
        public async Task<IActionResult> MomoReturn()
            => Ok(await _paymentService.HandleMomoReturnAsync(Request.Query));
    }
}
