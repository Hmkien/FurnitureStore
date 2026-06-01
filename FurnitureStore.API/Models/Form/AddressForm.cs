using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.API.Models.Form
{
    public class AddressForm
    {
        [Required(ErrorMessage = "Tên người nhận là bắt buộc")]
        [StringLength(150)]
        public string ReceiverName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(20)]
        public string ReceiverPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(500)]
        public string AddressLine { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Label { get; set; }

        public bool IsDefault { get; set; }
    }
}
