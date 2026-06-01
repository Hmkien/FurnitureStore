using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.ViewModels;
using FurnitureStore.API.Services.Base;

namespace FurnitureStore.API.Interface
{
    public interface IProductService : ICrudService<Product, ProductForm>
    {
        /// <summary>Chi tiết sản phẩm kèm danh mục, biến thể, ảnh.</summary>
        Task<ProductDetailVM> GetDetailAsync(Guid id);

        /// <summary>Các giá trị lọc khả dụng (chất liệu, màu, phong cách, khoảng giá).</summary>
        Task<ProductFilterOptionsVM> GetFilterOptionsAsync();
    }
}
