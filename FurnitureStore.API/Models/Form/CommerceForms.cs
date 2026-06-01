using System.ComponentModel.DataAnnotations;
using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.Form
{
    public class ReviewForm
    {
        [Required(ErrorMessage = "Sản phẩm là bắt buộc")]
        public Guid ProductId { get; set; }

        [Range(1, 5, ErrorMessage = "Đánh giá từ 1 đến 5 sao")]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        /// <summary>Mã OTP xác thực trước khi gửi đánh giá.</summary>
        [Required(ErrorMessage = "Vui lòng nhập mã OTP")]
        public string Otp { get; set; } = string.Empty;
    }

    public class CouponForm
    {
        [Required(ErrorMessage = "Mã giảm giá là bắt buộc")]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Description { get; set; }

        [Required]
        public DiscountType DiscountType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá trị giảm phải >= 0")]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MinOrderAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MaxDiscount { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Range(0, int.MaxValue)]
        public int UsageLimit { get; set; }
    }

    public class MembershipTierForm
    {
        [Required(ErrorMessage = "Tên hạng là bắt buộc")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Mức chi tiêu tối thiểu phải >= 0")]
        public decimal MinSpending { get; set; }

        [Range(0, 100, ErrorMessage = "Chiết khấu từ 0 đến 100%")]
        public decimal DiscountPercent { get; set; }

        [StringLength(300)]
        public string? Description { get; set; }
    }
}
