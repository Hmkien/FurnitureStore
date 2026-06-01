using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>Giỏ hàng của user đang đăng nhập.</summary>
        [HttpGet]
        public async Task<IActionResult> GetMyCart()
            => Ok(await _cartService.GetMyCartAsync());

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddToCartForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(await _cartService.AddItemAsync(form));
        }

        [HttpPut("items/{cartItemId}")]
        public async Task<IActionResult> UpdateItem(Guid cartItemId, [FromBody] UpdateCartItemForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(await _cartService.UpdateItemAsync(cartItemId, form.Quantity));
        }

        [HttpDelete("items/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(Guid cartItemId)
            => Ok(await _cartService.RemoveItemAsync(cartItemId));

        [HttpDelete]
        public async Task<IActionResult> Clear()
        {
            await _cartService.ClearAsync();
            return NoContent();
        }

        /// <summary>Gộp giỏ khách vào giỏ user sau khi đăng nhập.</summary>
        [Authorize]
        [HttpPost("merge")]
        public async Task<IActionResult> Merge([FromBody] MergeCartForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(await _cartService.MergeAsync(form.GuestToken));
        }
    }
}
