using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.ViewModels;

namespace FurnitureStore.API.Interface
{
    public interface ICartService
    {
        Task<CartVM> GetMyCartAsync();
        Task<CartVM> AddItemAsync(AddToCartForm form);
        Task<CartVM> UpdateItemAsync(Guid cartItemId, int quantity);
        Task<CartVM> RemoveItemAsync(Guid cartItemId);
        Task ClearAsync();

        /// <summary>Gộp giỏ khách (theo guestToken) vào giỏ của user đang đăng nhập.</summary>
        Task<CartVM> MergeAsync(string guestToken);
    }
}
