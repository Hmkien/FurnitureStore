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
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>Đánh giá + điểm trung bình của sản phẩm (storefront).</summary>
        [AllowAnonymous]
        [HttpGet("by-product/{productId}")]
        public async Task<IActionResult> GetByProduct(Guid productId)
            => Ok(await _reviewService.GetByProductAsync(productId));

        /// <summary>Danh sách đánh giá (quản trị, để duyệt/ẩn).</summary>
        [HttpPost("GetPaged")]
        public async Task<IActionResult> GetPaged([FromBody] BaseQuery query)
            => Ok(await _reviewService.GetPaged(query));

        /// <summary>Yêu cầu mã OTP trước khi gửi đánh giá (demo: trả OTP trực tiếp).</summary>
        [HttpPost("request-otp/{productId}")]
        public async Task<IActionResult> RequestOtp(Guid productId)
        {
            var otp = await _reviewService.RequestOtpAsync(productId);
            return Ok(new { otp, message = "Mã OTP có hiệu lực 5 phút" });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReviewForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _reviewService.Create(form);
            return StatusCode(StatusCodes.Status201Created, new { message = "Cảm ơn bạn đã đánh giá" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ReviewForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _reviewService.Update(id, form);
            return NoContent();
        }

        /// <summary>Duyệt hiển thị đánh giá (quản trị).</summary>
        [HttpPut("{id}/Approved")]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _reviewService.Approved(id);
            return NoContent();
        }

        /// <summary>Ẩn đánh giá (quản trị).</summary>
        [HttpPut("{id}/Reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            await _reviewService.Reject(id);
            return NoContent();
        }
    }
}
