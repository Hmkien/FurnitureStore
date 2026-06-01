using FurnitureStore.API.Interface;
using FurnitureStore.API.Middlewares;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.Query;
using FurnitureStore.API.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly RequestContext _requestContext;

        public UserController(IUserService userService, RequestContext requestContext)
        {
            _userService = userService;
            _requestContext = requestContext;
        }

        #region Query & Get

        /// <summary>
        /// Lấy danh sách user với phân trang, tìm kiếm, filter
        /// </summary>
        [HttpPost("GetPaged")]
        public async Task<IActionResult> GetPaged([FromBody] UserQuery query)
        {
            var result = await _userService.GetPaged(query);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin user hiện tại
        /// </summary>
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var currentUser = await _userService.GetCurrentUser();
            return Ok(currentUser);
        }

        /// <summary>
        /// Lấy thông tin user theo ID (detail)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserVMByIdAsync(id);
            if (user == null)
                return NotFound(new { error = "Không tìm thấy user" });

            return Ok(user);
        }

        #endregion

        #region Create, Update, Delete

        /// <summary>
        /// Tạo user mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] UserForm userForm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(userForm.Password))
                return BadRequest(new { error = "Mật khẩu là bắt buộc" });

            await _userService.CreateUserAsync(userForm);
            return StatusCode(StatusCodes.Status201Created, new { message = "Tạo user thành công" });
        }

        /// <summary>
        /// Cập nhật thông tin user
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UserUpdateForm userForm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = _requestContext.GetUserId();
            var canEdit = _requestContext.IsSuperUser()
                          || _requestContext.HasPermission("USER_EDIT")
                          || currentUserId == id;

            if (!canEdit)
                return Forbid("Bạn không có quyền cập nhật user này");

            var result = await _userService.Update(userForm, id);
            return result
                ? Ok(new { message = "Cập nhật user thành công" })
                : NotFound(new { error = "Không tìm thấy user" });
        }

        /// <summary>
        /// Xóa user
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Không cho phép tự xóa chính mình
            if (_requestContext.GetUserId() == id)
                return BadRequest(new { error = "Không thể xóa chính mình" });

            var result = await _userService.DeleteUserAsync(id);
            return result
                ? Ok(new { message = "Xóa user thành công" })
                : NotFound(new { error = "Không tìm thấy user" });
        }

        #endregion

        #region Password Management

        /// <summary>
        /// Đổi mật khẩu (user tự đổi)
        /// </summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestVM request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _requestContext.GetUserId();
            var result = await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
            return result
                ? Ok(new { message = "Đổi mật khẩu thành công" })
                : BadRequest(new { error = "Mật khẩu hiện tại không đúng" });
        }

        /// <summary>
        /// Admin đổi mật khẩu cho user
        /// </summary>
        [HttpPost("{userId}/reset-password")]
        public async Task<IActionResult> ResetPasswordByAdmin(Guid userId, [FromBody] ChangePasswordAdminRequestVM request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ChangePasswordByAdmin(userId, request);
            return result
                ? Ok(new { message = "Đổi mật khẩu thành công" })
                : NotFound(new { error = "Không tìm thấy user" });
        }

        #endregion

        #region Status Management

        /// <summary>
        /// Phê duyệt user
        /// </summary>
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var result = await _userService.Approved(id);
            return result
                ? Ok(new { message = "Phê duyệt user thành công" })
                : NotFound(new { error = "Không tìm thấy user" });
        }

        /// <summary>
        /// Từ chối user
        /// </summary>
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            var result = await _userService.Reject(id);
            return result
                ? Ok(new { message = "Từ chối user thành công" })
                : NotFound(new { error = "Không tìm thấy user" });
        }

        #endregion
    }
}
