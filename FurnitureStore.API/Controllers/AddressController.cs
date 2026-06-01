using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    /// <summary>Sổ địa chỉ giao hàng của người dùng đang đăng nhập.</summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMine()
            => Ok(await _addressService.GetMyAddressesAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddressForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(await _addressService.CreateAsync(form));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AddressForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _addressService.UpdateAsync(id, form);
            return Ok(new { message = "Đã cập nhật địa chỉ" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _addressService.DeleteAsync(id);
            return Ok(new { message = "Đã xóa địa chỉ" });
        }

        [HttpPut("{id}/default")]
        public async Task<IActionResult> SetDefault(Guid id)
        {
            await _addressService.SetDefaultAsync(id);
            return Ok(new { message = "Đã đặt làm địa chỉ mặc định" });
        }
    }
}
