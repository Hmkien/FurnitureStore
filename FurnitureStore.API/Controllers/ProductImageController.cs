using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageService _imageService;

        public ProductImageController(IProductImageService imageService)
        {
            _imageService = imageService;
        }

        [AllowAnonymous]
        [HttpGet("by-product/{productId}")]
        public async Task<IActionResult> GetByProduct(Guid productId)
            => Ok(await _imageService.GetByProductAsync(productId));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductImageForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _imageService.Create(form);
            return StatusCode(StatusCodes.Status201Created, new { message = "Tạo thành công" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductImageForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _imageService.Update(id, form);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _imageService.Delete(id);
            return NoContent();
        }
    }
}
