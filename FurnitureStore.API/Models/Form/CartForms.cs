using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.API.Models.Form
{
    public class AddToCartForm
    {
        [Required(ErrorMessage = "Biến thể sản phẩm là bắt buộc")]
        public Guid VariantId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải >= 1")]
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemForm
    {
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải >= 1")]
        public int Quantity { get; set; }
    }

    public class MergeCartForm
    {
        [Required(ErrorMessage = "guestToken là bắt buộc")]
        [StringLength(64)]
        public string GuestToken { get; set; } = string.Empty;
    }
}
