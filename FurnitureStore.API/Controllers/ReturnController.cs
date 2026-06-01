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
    public class ReturnController : ControllerBase
    {
        private readonly IReturnService _returnService;

        public ReturnController(IReturnService returnService)
        {
            _returnService = returnService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReturnForm form)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _returnService.CreateAsync(form);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost("GetPaged")]
        public async Task<IActionResult> GetAll([FromBody] BaseQuery query) => Ok(await _returnService.GetAllAsync(query));

        [HttpPost("my/GetPaged")]
        public async Task<IActionResult> GetMy([FromBody] BaseQuery query) => Ok(await _returnService.GetMyAsync(query));

        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(Guid id) => Ok(await _returnService.GetDetailAsync(id));

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateReturnStatusForm form)
        {
            await _returnService.UpdateStatusAsync(id, form.Status, form.Note);
            return NoContent();
        }
    }
}
