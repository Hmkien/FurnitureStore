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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
            => Ok(await _categoryService.GetByIdAsync(id));

        [AllowAnonymous]
        [HttpPost("GetPaged")]
        public async Task<IActionResult> GetPaged([FromBody] BaseQuery query)
            => Ok(await _categoryService.GetPaged(query));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _categoryService.Create(form);
            return StatusCode(StatusCodes.Status201Created, new { message = "Tạo thành công" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CategoryForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _categoryService.Update(id, form);
            return NoContent();
        }

        [HttpPut("{id}/Approved")]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _categoryService.Approved(id);
            return NoContent();
        }

        [HttpPut("{id}/Reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            await _categoryService.Reject(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _categoryService.Delete(id);
            return NoContent();
        }
    }
}
