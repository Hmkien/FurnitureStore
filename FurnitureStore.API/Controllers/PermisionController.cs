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
    public class PermisionController : ControllerBase
    {
        private readonly IPermisionService _permisionService;

        public PermisionController(IPermisionService permisionService)
        {
            _permisionService = permisionService;
        }

        /// <summary>
        /// Lấy thông tin Permision theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var permision = await _permisionService.GetByIdAsync(id);
            return Ok(permision);
        }

        /// <summary>
        /// Lấy danh sách Permision với phân trang
        /// </summary>
        [HttpPost("GetPaged")]
        public async Task<IActionResult> GetPaged([FromBody] BaseQuery query)
        {
            var result = await _permisionService.GetPaged(query);
            return Ok(result);
        }

        /// <summary>
        /// Tạo Permision mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PermisionForm permisionForm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _permisionService.Create(permisionForm);
            return StatusCode(StatusCodes.Status201Created, new { message = "Tạo thành công" });
        }

        /// <summary>
        /// Cập nhật Permision
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PermisionForm permisionForm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _permisionService.Update(id, permisionForm);
            return NoContent();
        }

        /// <summary>
        /// Phê duyệt Permision
        /// </summary>
        [HttpPut("{id}/Approved")]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _permisionService.Approved(id);
            return NoContent();
        }

        /// <summary>
        /// Từ chối Permision
        /// </summary>
        [HttpPut("{id}/Reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            await _permisionService.Reject(id);
            return NoContent();
        }
    }
}
