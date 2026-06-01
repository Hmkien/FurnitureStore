namespace FurnitureStore.API.Models.ViewModels
{
    /// <summary>
    /// Mục thông báo cho admin (đơn hàng mới, đánh giá mới...).
    /// Tổng hợp từ sự kiện gần đây, không lưu bảng riêng.
    /// </summary>
    public class NotificationVM
    {
        /// <summary>"order" | "review"</summary>
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        /// <summary>Đường dẫn điều hướng ở frontend (nếu có).</summary>
        public string? Link { get; set; }
    }
}
