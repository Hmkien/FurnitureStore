using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Services.Base;

namespace FurnitureStore.API.Interface
{
    public interface IPostService : ICrudService<Post, PostForm>
    {
        /// <summary>Lấy bài viết đã xuất bản theo slug (storefront).</summary>
        Task<Post> GetBySlugAsync(string slug);
    }

    public interface IBannerService : ICrudService<Banner, BannerForm>
    {
        /// <summary>Banner đang hoạt động (trong thời gian hiệu lực), tùy chọn theo vị trí.</summary>
        Task<List<Banner>> GetActiveAsync(string? position);
    }

    public interface ISlideService : ICrudService<Slide, SlideForm>
    {
        /// <summary>Slide đang hoạt động, sắp theo thứ tự.</summary>
        Task<List<Slide>> GetActiveAsync();
    }
}
