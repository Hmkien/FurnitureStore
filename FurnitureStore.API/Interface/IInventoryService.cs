using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.Query;
using FurnitureStore.API.Models.ViewModels;

namespace FurnitureStore.API.Interface
{
    /// <summary>Quản lý kho: nhập/xuất/điều chỉnh tồn kho và xem lịch sử.</summary>
    public interface IInventoryService
    {
        /// <summary>Áp dụng một biến động kho, cập nhật tồn và ghi lịch sử.</summary>
        Task<ProductVariant> AdjustAsync(StockAdjustForm form);

        /// <summary>Lịch sử biến động của một biến thể (phân trang).</summary>
        Task<DataTableJson> GetHistoryAsync(Guid variantId, BaseQuery query);

        /// <summary>Danh sách biến thể có tồn kho dưới ngưỡng.</summary>
        Task<List<LowStockVariantVM>> GetLowStockAsync(int threshold);
    }
}
