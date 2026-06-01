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
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
            => Ok(await _postService.GetByIdAsync(id));

        [AllowAnonymous]
        [HttpGet("by-slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
            => Ok(await _postService.GetBySlugAsync(slug));

        [AllowAnonymous]
        [HttpPost("GetPaged")]
        public async Task<IActionResult> GetPaged([FromBody] BaseQuery query)
            => Ok(await _postService.GetPaged(query));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PostForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _postService.Create(form);
            return StatusCode(StatusCodes.Status201Created, new { message = "Tạo thành công" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PostForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _postService.Update(id, form);
            return NoContent();
        }

        [HttpPut("{id}/Approved")]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _postService.Approved(id);
            return NoContent();
        }

        [HttpPut("{id}/Reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            await _postService.Reject(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _postService.Delete(id);
            return NoContent();
        }
    }
}
