using System.ComponentModel.DataAnnotations;
using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.Form
{
    public class ReturnItemForm
    {
        [Required]
        public Guid VariantId { get; set; }
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
    }

    public class CreateReturnForm
    {
        [Required(ErrorMessage = "Đơn hàng là bắt buộc")]
        public Guid OrderId { get; set; }

        [Required]
        public ReturnType Type { get; set; }

        [Required(ErrorMessage = "Lý do là bắt buộc")]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [MinLength(1, ErrorMessage = "Chọn ít nhất 1 sản phẩm")]
        public List<ReturnItemForm> Items { get; set; } = new();
    }

    public class UpdateReturnStatusForm
    {
        [Required]
        public ReturnStatus Status { get; set; }
        [StringLength(500)]
        public string? Note { get; set; }
    }

    public class CreateWarrantyForm
    {
        [Required(ErrorMessage = "Đơn hàng là bắt buộc")]
        public Guid OrderId { get; set; }

        [Required(ErrorMessage = "Sản phẩm là bắt buộc")]
        public Guid VariantId { get; set; }

        [Required(ErrorMessage = "Tên khách là bắt buộc")]
        [StringLength(150)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(20)]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả lỗi là bắt buộc")]
        [StringLength(1000)]
        public string IssueDescription { get; set; } = string.Empty;
    }

    public class UpdateWarrantyStatusForm
    {
        [Required]
        public WarrantyStatus Status { get; set; }
        [StringLength(500)]
        public string? Note { get; set; }
    }
}
