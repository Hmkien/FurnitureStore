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
    public class MembershipTierController : ControllerBase
    {
        private readonly IMembershipTierService _tierService;

        public MembershipTierController(IMembershipTierService tierService)
        {
            _tierService = tierService;
        }

        /// <summary>Hạng thành viên hiện tại của tôi.</summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMyMembership()
            => Ok(await _tierService.GetMyMembershipAsync());

        [AllowAnonymous]
        [HttpPost("GetPaged")]
        public async Task<IActionResult> GetPaged([FromBody] BaseQuery query)
            => Ok(await _tierService.GetPaged(query));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MembershipTierForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _tierService.Create(form);
            return StatusCode(StatusCodes.Status201Created, new { message = "Tạo thành công" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MembershipTierForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _tierService.Update(id, form);
            return NoContent();
        }

        [HttpPut("{id}/Approved")]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _tierService.Approved(id);
            return NoContent();
        }

        [HttpPut("{id}/Reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            await _tierService.Reject(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _tierService.Delete(id);
            return NoContent();
        }
    }
}
