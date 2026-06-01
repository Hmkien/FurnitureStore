using FurnitureStore.API.Models.ViewModels;

namespace FurnitureStore.API.Interface
{
    public interface INotificationService
    {
        /// <summary>Các thông báo gần đây (đơn hàng mới + đánh giá mới).</summary>
        Task<List<NotificationVM>> GetRecentAsync(int take);
    }
}
