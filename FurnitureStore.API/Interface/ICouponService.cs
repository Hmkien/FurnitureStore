using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.ViewModels;
using FurnitureStore.API.Services.Base;

namespace FurnitureStore.API.Interface
{
    public interface ICouponService : ICrudService<Coupon, CouponForm>
    {
        /// <summary>Danh sách mã giảm giá đang hoạt động (công khai cho storefront).</summary>
        Task<List<CouponPublicVM>> GetActiveAsync();

        /// <summary>Xem trước mức giảm cho một mã + giá trị đơn (không thay đổi dữ liệu).</summary>
        Task<CouponPreviewVM> PreviewAsync(string code, decimal subTotal);

        /// <summary>
        /// Kiểm tra hợp lệ và tính mức giảm. Trả về coupon (đang được theo dõi
        /// bởi DbContext) để nơi gọi tăng UsedCount khi lưu đơn.
        /// </summary>
        Task<(Coupon coupon, decimal discount)> ValidateAndComputeAsync(string code, decimal subTotal);
    }
}
