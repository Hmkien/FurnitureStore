using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.API.Models.ViewModels
{
    public class RefreshTokenRequestVM
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}

