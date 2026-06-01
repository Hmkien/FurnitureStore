using FurnitureStore.API.Models.ViewModels;

namespace FurnitureStore.API.Interface
{
    /// <summary>
    /// Phân quyền (RBAC): đồng bộ quyền/vai trò từ enum, gán quyền cho vai trò,
    /// và gán vai trò cho người dùng.
    /// </summary>
    public interface IRbacService
    {
        /// <summary>Tạo/đồng bộ toàn bộ quyền và vai trò mặc định từ enum + map quyền cho vai trò.</summary>
        Task<RbacSyncResultVM> SyncAsync();

        /// <summary>Tất cả quyền (đã gom nhóm theo module) để hiển thị màn hình phân quyền.</summary>
        Task<List<PermissionOptionVM>> GetAllPermissionsAsync();

        /// <summary>Tất cả vai trò để gán cho người dùng.</summary>
        Task<List<RoleOptionVM>> GetAllRolesAsync();

        /// <summary>Id các quyền đang gán cho một vai trò.</summary>
        Task<List<Guid>> GetRolePermissionIdsAsync(Guid roleId);

        /// <summary>Thay thế toàn bộ quyền của một vai trò.</summary>
        Task SetRolePermissionsAsync(Guid roleId, List<Guid> permissionIds);

        /// <summary>Id các vai trò đang gán cho một người dùng.</summary>
        Task<List<Guid>> GetUserRoleIdsAsync(Guid userId);

        /// <summary>Thay thế toàn bộ vai trò của một người dùng.</summary>
        Task SetUserRolesAsync(Guid userId, List<Guid> roleIds);
    }
}
