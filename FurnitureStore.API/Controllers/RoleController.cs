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
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Lấy thông tin Role theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var role = await _roleService.GetByIdAsync(id);
            return Ok(role);
        }

        /// <summary>
        /// Lấy danh sách Role với phân trang
        /// </summary>
        [HttpPost("GetPaged")]
        public async Task<IActionResult> GetPaged([FromBody] BaseQuery query)
        {
            var result = await _roleService.GetPaged(query);
            return Ok(result);
        }

        /// <summary>
        /// Tạo Role mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleForm roleForm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _roleService.Create(roleForm);
            return StatusCode(StatusCodes.Status201Created, new { message = "Tạo thành công" });
        }

        /// <summary>
        /// Cập nhật Role
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] RoleForm roleForm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _roleService.Update(id, roleForm);
            return NoContent();
        }

        /// <summary>
        /// Phê duyệt Role
        /// </summary>
        [HttpPut("{id}/Approved")]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _roleService.Approved(id);
            return NoContent();
        }

        /// <summary>
        /// Từ chối Role
        /// </summary>
        [HttpPut("{id}/Reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            await _roleService.Reject(id);
            return NoContent();
        }
    }
}
