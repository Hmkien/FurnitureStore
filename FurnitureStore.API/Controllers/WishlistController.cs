using FurnitureStore.API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        /// <summary>Danh sách yêu thích của tôi.</summary>
        [HttpGet]
        public async Task<IActionResult> GetMine()
            => Ok(await _wishlistService.GetMineAsync());

        /// <summary>Bật/tắt một sản phẩm trong danh sách yêu thích.</summary>
        [HttpPost("toggle/{productId}")]
        public async Task<IActionResult> Toggle(Guid productId)
            => Ok(new { inWishlist = await _wishlistService.ToggleAsync(productId) });
    }
}
