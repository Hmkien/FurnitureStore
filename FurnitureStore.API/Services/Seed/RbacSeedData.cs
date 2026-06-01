using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Services.Seed
{
    /// <summary>
    /// Định nghĩa vai trò mặc định và tập quyền của từng vai trò.
    /// Dùng để seed/đồng bộ RBAC từ enum (xem RbacService.SyncAsync).
    /// </summary>
    public static class RbacSeedData
    {
        /// <summary>Tên hiển thị tiếng Việt cho từng vai trò.</summary>
        public static readonly Dictionary<DefaultRole, string> RoleNames = new()
        {
            [DefaultRole.ADMIN] = "Quản trị viên",
            [DefaultRole.MANAGER] = "Quản lý cửa hàng",
            [DefaultRole.SALE] = "Nhân viên bán hàng",
            [DefaultRole.CONTENT] = "Biên tập nội dung",
            [DefaultRole.CUSTOMER] = "Khách hàng",
        };

        /// <summary>Tập quyền của từng vai trò. ADMIN nhận tất cả (xử lý riêng khi seed).</summary>
        public static readonly Dictionary<DefaultRole, PermissionCode[]> RolePermissions = new()
        {
            [DefaultRole.ADMIN] = AllPermissions,

            [DefaultRole.MANAGER] = new[]
            {
                PermissionCode.REPORT_VIEW,
                PermissionCode.PRODUCT_VIEW, PermissionCode.PRODUCT_MANAGE, PermissionCode.CATEGORY_MANAGE,
                PermissionCode.INVENTORY_MANAGE,
                PermissionCode.ORDER_VIEW, PermissionCode.ORDER_MANAGE,
                PermissionCode.RETURN_MANAGE, PermissionCode.WARRANTY_MANAGE,
                PermissionCode.COUPON_MANAGE, PermissionCode.MEMBERSHIP_MANAGE,
                PermissionCode.REVIEW_MANAGE,
            },

            [DefaultRole.SALE] = new[]
            {
                PermissionCode.PRODUCT_VIEW,
                PermissionCode.ORDER_VIEW, PermissionCode.ORDER_MANAGE,
                PermissionCode.RETURN_MANAGE, PermissionCode.WARRANTY_MANAGE,
            },

            [DefaultRole.CONTENT] = new[]
            {
                PermissionCode.PRODUCT_VIEW,
                PermissionCode.CONTENT_MANAGE, PermissionCode.REVIEW_MANAGE,
            },

            [DefaultRole.CUSTOMER] = Array.Empty<PermissionCode>(),
        };

        private static PermissionCode[] AllPermissions =>
            Enum.GetValues<PermissionCode>();
    }
}
