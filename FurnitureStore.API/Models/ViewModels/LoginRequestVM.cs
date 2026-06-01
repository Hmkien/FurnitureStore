using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.API.Models.ViewModels
{
    public class LoginRequestVM
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = string.Empty;
        public bool RememberPassword { get; set; } = false;
    }
}

