using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.ViewModels;
using FurnitureStore.API.Services.Base;

namespace FurnitureStore.API.Interface
{
    public interface IReviewService : ICrudService<Review, ReviewForm>
    {
        /// <summary>Danh sách đánh giá đã duyệt + điểm trung bình của sản phẩm.</summary>
        Task<ProductReviewSummaryVM> GetByProductAsync(Guid productId);

        /// <summary>Phát hành mã OTP để khách xác thực trước khi gửi đánh giá.</summary>
        Task<string> RequestOtpAsync(Guid productId);
    }
}
