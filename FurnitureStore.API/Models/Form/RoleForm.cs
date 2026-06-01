using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.API.Models.Form
{
    public class RoleForm
    {
        [Required(ErrorMessage = "Name là bắt buộc.")]
        [StringLength(200, ErrorMessage = "Name không được vượt quá 200 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "RoleCode là bắt buộc.")]
        [StringLength(200, ErrorMessage = "RoleCode không được vượt quá 200 ký tự.")]
        public string RoleCode { get; set; } = string.Empty;
    }
}

