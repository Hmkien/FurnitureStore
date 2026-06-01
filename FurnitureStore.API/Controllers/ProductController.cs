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
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
            => Ok(await _productService.GetByIdAsync(id));

        /// <summary>Chi tiết sản phẩm kèm danh mục, biến thể, ảnh (cho storefront).</summary>
        [AllowAnonymous]
        [HttpGet("{id}/detail")]
        public async Task<IActionResult> GetDetail(Guid id)
            => Ok(await _productService.GetDetailAsync(id));

        /// <summary>Danh sách sản phẩm với phân trang + bộ lọc (danh mục, giá, chất liệu, phong cách, tình trạng).</summary>
        [AllowAnonymous]
        [HttpPost("GetPaged")]
        public async Task<IActionResult> GetPaged([FromBody] ProductQuery query)
            => Ok(await _productService.GetPaged(query));

        /// <summary>Các giá trị lọc khả dụng cho storefront.</summary>
        [AllowAnonymous]
        [HttpGet("filter-options")]
        public async Task<IActionResult> GetFilterOptions()
            => Ok(await _productService.GetFilterOptionsAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _productService.Create(form);
            return StatusCode(StatusCodes.Status201Created, new { message = "Tạo thành công" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _productService.Update(id, form);
            return NoContent();
        }

        [HttpPut("{id}/Approved")]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _productService.Approved(id);
            return NoContent();
        }

        [HttpPut("{id}/Reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            await _productService.Reject(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productService.Delete(id);
            return NoContent();
        }
    }
}
