using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Query;
using FurnitureStore.API.Models.ViewModels;

namespace FurnitureStore.API.Interface
{
    /// <summary>Kho media: upload, liệt kê và xóa file ảnh dùng chung.</summary>
    public interface IMediaService
    {
        Task<MediaFile> UploadAsync(IFormFile file, string? folder);
        Task<DataTableJson> GetPaged(BaseQuery query);
        Task<bool> DeleteAsync(Guid id);
    }
}
