using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.API.Models.Form
{
    public class UserUpdateForm
    {

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255, ErrorMessage = "Email tối đa 255 ký tự")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Họ tối đa 100 ký tự")]
        public string? FirstName { get; set; }

        [StringLength(100, ErrorMessage = "Tên tối đa 100 ký tự")]
        public string? LastName { get; set; }

        public DateTime? Birthday { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự")]
        public string? PhoneNumber { get; set; }

        [StringLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự")]
        public string? Address { get; set; }
        public IFormFile? ImageAvatar { get; set; }
        public bool RemoveImage { get; set; }
    }
}

