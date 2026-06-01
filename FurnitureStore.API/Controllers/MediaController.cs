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
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;

        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        /// <summary>Upload một ảnh vào kho media, trả về metadata + URL dùng lại.</summary>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] MediaUploadForm form)
        {
            var media = await _mediaService.UploadAsync(form.File, form.Folder);
            return StatusCode(StatusCodes.Status201Created, media);
        }

        [HttpPost("GetPaged")]
        public async Task<IActionResult> GetPaged([FromBody] BaseQuery query)
            => Ok(await _mediaService.GetPaged(query));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _mediaService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound(new { error = "Không tìm thấy file" });
        }
    }
}
