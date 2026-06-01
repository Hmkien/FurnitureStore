using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Services.Base;

namespace FurnitureStore.API.Interface
{
    public interface IProductImageService : ICrudService<ProductImage, ProductImageForm>
    {
        /// <summary>Lấy tất cả ảnh của một sản phẩm.</summary>
        Task<List<ProductImage>> GetByProductAsync(Guid productId);
    }
}
