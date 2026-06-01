using FurnitureStore.API.Models.ViewModels;

namespace FurnitureStore.API.Interface
{
    public interface IWishlistService
    {
        /// <summary>Danh sách yêu thích của user đang đăng nhập.</summary>
        Task<List<WishlistItemVM>> GetMineAsync();

        /// <summary>Bật/tắt một sản phẩm trong danh sách yêu thích. Trả về true nếu vừa được thêm.</summary>
        Task<bool> ToggleAsync(Guid productId);
    }
}
