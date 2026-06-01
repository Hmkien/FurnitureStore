using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariantController : ControllerBase
    {
        private readonly IProductVariantService _variantService;

        public ProductVariantController(IProductVariantService variantService)
        {
            _variantService = variantService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
            => Ok(await _variantService.GetByIdAsync(id));

        [AllowAnonymous]
        [HttpGet("by-product/{productId}")]
        public async Task<IActionResult> GetByProduct(Guid productId)
            => Ok(await _variantService.GetByProductAsync(productId));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductVariantForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _variantService.Create(form);
            return StatusCode(StatusCodes.Status201Created, new { message = "Tạo thành công" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductVariantForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _variantService.Update(id, form);
            return NoContent();
        }

        [HttpPut("{id}/Approved")]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _variantService.Approved(id);
            return NoContent();
        }

        [HttpPut("{id}/Reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            await _variantService.Reject(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _variantService.Delete(id);
            return NoContent();
        }
    }
}
