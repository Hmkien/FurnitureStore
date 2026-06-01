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
    public class BannerController : ControllerBase
    {
        private readonly IBannerService _bannerService;

        public BannerController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        /// <summary>Banner đang hoạt động (storefront), lọc theo vị trí tùy chọn.</summary>
        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActive([FromQuery] string? position)
            => Ok(await _bannerService.GetActiveAsync(position));

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
            => Ok(await _bannerService.GetByIdAsync(id));

        [HttpPost("GetPaged")]
        public async Task<IActionResult> GetPaged([FromBody] BaseQuery query)
            => Ok(await _bannerService.GetPaged(query));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BannerForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _bannerService.Create(form);
            return StatusCode(StatusCodes.Status201Created, new { message = "Tạo thành công" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BannerForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _bannerService.Update(id, form);
            return NoContent();
        }

        [HttpPut("{id}/Approved")]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _bannerService.Approved(id);
            return NoContent();
        }

        [HttpPut("{id}/Reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            await _bannerService.Reject(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _bannerService.Delete(id);
            return NoContent();
        }
    }
}
