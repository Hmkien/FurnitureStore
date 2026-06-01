using System.ComponentModel.DataAnnotations;
using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.Form
{
    public class StockAdjustForm
    {
        [Required(ErrorMessage = "Biến thể là bắt buộc")]
        public Guid VariantId { get; set; }

        [Required]
        public StockMovementType Type { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải >= 1")]
        public int Quantity { get; set; }

        [StringLength(300)]
        public string? Note { get; set; }
    }
}
