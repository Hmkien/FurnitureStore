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
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        /// <summary>Danh sách mã giảm giá đang hoạt động — công khai cho storefront.</summary>
        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
            => Ok(await _couponService.GetActiveAsync());

        /// <summary>Xem trước mức giảm cho mã + giá trị đơn (dùng ở giỏ hàng).</summary>
        [HttpGet("preview")]
        public async Task<IActionResult> Preview([FromQuery] string code, [FromQuery] decimal subTotal)
            => Ok(await _couponService.PreviewAsync(code, subTotal));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
            => Ok(await _couponService.GetByIdAsync(id));

        [HttpPost("GetPaged")]
        public async Task<IActionResult> GetPaged([FromBody] BaseQuery query)
            => Ok(await _couponService.GetPaged(query));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CouponForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _couponService.Create(form);
            return StatusCode(StatusCodes.Status201Created, new { message = "Tạo thành công" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CouponForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _couponService.Update(id, form);
            return NoContent();
        }

        [HttpPut("{id}/Approved")]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _couponService.Approved(id);
            return NoContent();
        }

        [HttpPut("{id}/Reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            await _couponService.Reject(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _couponService.Delete(id);
            return NoContent();
        }
    }
}
