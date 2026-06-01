namespace FurnitureStore.API.Models.ViewModels
{
    /// <summary>Một quyền dùng cho màn hình phân quyền (kèm nhóm module để gom nhóm).</summary>
    public class PermissionOptionVM
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Module { get; set; } = "Khác";
    }

    /// <summary>Một vai trò dùng cho dropdown/checkbox phân vai trò.</summary>
    public class RoleOptionVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
    }

    /// <summary>Kết quả đồng bộ RBAC từ enum.</summary>
    public class RbacSyncResultVM
    {
        public int PermissionsCreated { get; set; }
        public int RolesCreated { get; set; }
        public int MappingsCreated { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
