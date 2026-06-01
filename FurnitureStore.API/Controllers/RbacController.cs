using FurnitureStore.API.Interface;
using FurnitureStore.API.Middlewares;
using FurnitureStore.API.Models.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    /// <summary>
    /// Phân quyền: đồng bộ quyền/vai trò từ enum, gán quyền cho vai trò, gán vai trò cho người dùng.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RbacController : ControllerBase
    {
        private readonly IRbacService _rbac;

        public RbacController(IRbacService rbac)
        {
            _rbac = rbac;
        }

        /// <summary>Tạo/đồng bộ toàn bộ quyền + vai trò mặc định từ enum (chỉ SuperUser hoặc quyền PERMISSION_MANAGE).</summary>
        [HttpPost("sync")]
        [RequirePermission("PERMISSION_MANAGE")]
        public async Task<IActionResult> Sync()
        {
            var result = await _rbac.SyncAsync();
            return Ok(result);
        }

        /// <summary>Tất cả quyền (gom nhóm theo module) cho màn hình phân quyền.</summary>
        [HttpGet("permissions")]
        [RequirePermission("ROLE_MANAGE", "PERMISSION_MANAGE")]
        public async Task<IActionResult> GetPermissions()
            => Ok(await _rbac.GetAllPermissionsAsync());

        /// <summary>Tất cả vai trò để gán cho người dùng.</summary>
        [HttpGet("roles")]
        [RequirePermission("ROLE_MANAGE", "USER_MANAGE")]
        public async Task<IActionResult> GetRoles()
            => Ok(await _rbac.GetAllRolesAsync());

        /// <summary>Id các quyền của một vai trò.</summary>
        [HttpGet("roles/{roleId}/permissions")]
        [RequirePermission("ROLE_MANAGE")]
        public async Task<IActionResult> GetRolePermissions(Guid roleId)
            => Ok(await _rbac.GetRolePermissionIdsAsync(roleId));

        /// <summary>Gán (thay thế) quyền cho một vai trò.</summary>
        [HttpPut("roles/{roleId}/permissions")]
        [RequirePermission("ROLE_MANAGE")]
        public async Task<IActionResult> SetRolePermissions(Guid roleId, [FromBody] SetRolePermissionsForm form)
        {
            await _rbac.SetRolePermissionsAsync(roleId, form.PermissionIds);
            return Ok(new { message = "Đã cập nhật quyền cho vai trò" });
        }

        /// <summary>Id các vai trò của một người dùng.</summary>
        [HttpGet("users/{userId}/roles")]
        [RequirePermission("USER_MANAGE")]
        public async Task<IActionResult> GetUserRoles(Guid userId)
            => Ok(await _rbac.GetUserRoleIdsAsync(userId));

        /// <summary>Gán (thay thế) vai trò cho một người dùng.</summary>
        [HttpPut("users/{userId}/roles")]
        [RequirePermission("USER_MANAGE")]
        public async Task<IActionResult> SetUserRoles(Guid userId, [FromBody] SetUserRolesForm form)
        {
            await _rbac.SetUserRolesAsync(userId, form.RoleIds);
            return Ok(new { message = "Đã cập nhật vai trò cho người dùng" });
        }
    }
}
