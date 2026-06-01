using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.API.Models.Form
{
    public class PermisionForm
    {
        [Required(ErrorMessage = "PermisionName là bắt buộc.")]
        [StringLength(200, ErrorMessage = "PermisionName không được vượt quá 200 ký tự.")]
        public string PermisionName { get; set; } = string.Empty;

        [Required(ErrorMessage = "PermisionCode là bắt buộc.")]
        [StringLength(200, ErrorMessage = "PermisionCode không được vượt quá 200 ký tự.")]
        public string PermisionCode { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}

