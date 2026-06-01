namespace FurnitureStore.API.Models.Enums
{
    /// <summary>
    /// Metadata cho từng quyền: tên hiển thị + nhóm module (để gom nhóm trên UI phân quyền).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class PermissionMetaAttribute : Attribute
    {
        public string Name { get; }
        public string Module { get; }

        public PermissionMetaAttribute(string name, string module)
        {
            Name = name;
            Module = module;
        }
    }

    /// <summary>
    /// Danh mục quyền của hệ thống. <b>Tên enum chính là PermisionCode</b> lưu trong DB.
    /// Là nguồn chân lý duy nhất để seed/đồng bộ quyền (xem RbacService.SyncAsync).
    /// Các mã ORDER_MANAGE, INVENTORY_MANAGE, REPORT_VIEW, USER_EDIT đang được dùng
    /// trong code kiểm tra quyền nên bắt buộc phải có.
    /// </summary>
    public enum PermissionCode
    {
        [PermissionMeta("Xem báo cáo & thống kê", "Báo cáo")]
        REPORT_VIEW,

        [PermissionMeta("Xem sản phẩm", "Sản phẩm")]
        PRODUCT_VIEW,
        [PermissionMeta("Quản lý sản phẩm", "Sản phẩm")]
        PRODUCT_MANAGE,
        [PermissionMeta("Quản lý danh mục", "Sản phẩm")]
        CATEGORY_MANAGE,
        [PermissionMeta("Quản lý kho hàng", "Sản phẩm")]
        INVENTORY_MANAGE,

        [PermissionMeta("Xem đơn hàng", "Đơn hàng")]
        ORDER_VIEW,
        [PermissionMeta("Quản lý đơn hàng", "Đơn hàng")]
        ORDER_MANAGE,
        [PermissionMeta("Quản lý đổi / trả", "Đơn hàng")]
        RETURN_MANAGE,
        [PermissionMeta("Quản lý bảo hành", "Đơn hàng")]
        WARRANTY_MANAGE,

        [PermissionMeta("Quản lý mã giảm giá", "Khuyến mãi")]
        COUPON_MANAGE,
        [PermissionMeta("Quản lý hạng thành viên", "Khuyến mãi")]
        MEMBERSHIP_MANAGE,

        [PermissionMeta("Quản lý nội dung (tin bài / banner / slide / media)", "Nội dung")]
        CONTENT_MANAGE,
        [PermissionMeta("Quản lý đánh giá", "Nội dung")]
        REVIEW_MANAGE,

        [PermissionMeta("Xem tài khoản", "Hệ thống")]
        USER_VIEW,
        [PermissionMeta("Sửa tài khoản", "Hệ thống")]
        USER_EDIT,
        [PermissionMeta("Quản lý tài khoản (tạo / xóa / khóa / cấp vai trò)", "Hệ thống")]
        USER_MANAGE,
        [PermissionMeta("Quản lý vai trò & phân quyền", "Hệ thống")]
        ROLE_MANAGE,
        [PermissionMeta("Quản lý danh mục quyền", "Hệ thống")]
        PERMISSION_MANAGE,
    }
}
