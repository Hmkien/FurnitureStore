namespace FurnitureStore.API.Models.Form
{
    /// <summary>Gán (thay thế toàn bộ) danh sách quyền cho một vai trò.</summary>
    public class SetRolePermissionsForm
    {
        public List<Guid> PermissionIds { get; set; } = new();
    }

    /// <summary>Gán (thay thế toàn bộ) danh sách vai trò cho một người dùng.</summary>
    public class SetUserRolesForm
    {
        public List<Guid> RoleIds { get; set; } = new();
    }
}
