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
    public class WarrantyController : ControllerBase
    {
        private readonly IWarrantyService _warrantyService;

        public WarrantyController(IWarrantyService warrantyService)
        {
            _warrantyService = warrantyService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWarrantyForm form)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _warrantyService.CreateAsync(form);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost("GetPaged")]
        public async Task<IActionResult> GetAll([FromBody] BaseQuery query) => Ok(await _warrantyService.GetAllAsync(query));

        [HttpPost("my/GetPaged")]
        public async Task<IActionResult> GetMy([FromBody] BaseQuery query) => Ok(await _warrantyService.GetMyAsync(query));

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateWarrantyStatusForm form)
        {
            await _warrantyService.UpdateStatusAsync(id, form.Status, form.Note);
            return NoContent();
        }
    }
}
