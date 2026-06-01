namespace FurnitureStore.API.Models.Enums
{
    /// <summary>
    /// Vai trò mặc định để seed. <b>Tên enum chính là RoleCode</b> lưu trong DB.
    /// </summary>
    public enum DefaultRole
    {
        /// <summary>Toàn quyền vận hành (không phải SuperUser hạ tầng nhưng có mọi quyền nghiệp vụ).</summary>
        ADMIN,

        /// <summary>Quản lý cửa hàng: sản phẩm, kho, đơn hàng, khuyến mãi, báo cáo.</summary>
        MANAGER,

        /// <summary>Nhân viên bán hàng: xử lý đơn, đổi/trả, bảo hành.</summary>
        SALE,

        /// <summary>Biên tập nội dung: tin bài, banner, slide, media, đánh giá.</summary>
        CONTENT,

        /// <summary>Khách hàng (mua hàng trên storefront), không có quyền quản trị.</summary>
        CUSTOMER,
    }
}
