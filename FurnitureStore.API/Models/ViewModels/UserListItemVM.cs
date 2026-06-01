using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.ViewModels
{
    /// <summary>
    /// Dòng dữ liệu user cho danh sách phân trang. Giữ nguyên tên field như entity
    /// nhưng loại bỏ PasswordHash để không lộ ra ngoài API.
    /// </summary>
    public class UserListItemVM
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool IsSuperUser { get; set; }
        public string? ImageAvatar { get; set; }
        public StatusEntity Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
    }
}
