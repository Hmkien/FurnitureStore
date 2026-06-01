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
    public class SlideController : ControllerBase
    {
        private readonly ISlideService _slideService;

        public SlideController(ISlideService slideService)
        {
            _slideService = slideService;
        }

        /// <summary>Slide đang hoạt động cho slider trang chủ.</summary>
        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
            => Ok(await _slideService.GetActiveAsync());

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
            => Ok(await _slideService.GetByIdAsync(id));

        [HttpPost("GetPaged")]
        public async Task<IActionResult> GetPaged([FromBody] BaseQuery query)
            => Ok(await _slideService.GetPaged(query));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SlideForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _slideService.Create(form);
            return StatusCode(StatusCodes.Status201Created, new { message = "Tạo thành công" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SlideForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _slideService.Update(id, form);
            return NoContent();
        }

        [HttpPut("{id}/Approved")]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _slideService.Approved(id);
            return NoContent();
        }

        [HttpPut("{id}/Reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            await _slideService.Reject(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _slideService.Delete(id);
            return NoContent();
        }
    }
}
