using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Services.Base;

namespace FurnitureStore.API.Interface
{
    public interface IProductVariantService : ICrudService<ProductVariant, ProductVariantForm>
    {
        /// <summary>Lấy tất cả biến thể của một sản phẩm.</summary>
        Task<List<ProductVariant>> GetByProductAsync(Guid productId);
    }
}
