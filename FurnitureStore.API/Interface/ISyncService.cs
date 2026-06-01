namespace FurnitureStore.API.Interface
{
    /// <summary>
    /// Sinh dữ liệu mẫu ngẫu nhiên cho các menu quản lý (chỉ bật khi cấu hình EnableSync = true).
    /// </summary>
    public interface ISyncService
    {
        bool Enabled { get; }

        /// <summary>Tạo <paramref name="count"/> bản ghi ngẫu nhiên cho một resource.</summary>
        Task<int> GenerateAsync(string resource, int count);

        /// <summary>Seed sản phẩm thật từ moho.com.vn (mỗi lần ra sản phẩm khác nhau). Mặc định 5/lần.</summary>
        Task<int> SeedMohoProductsAsync(int count);

        /// <summary>Tạo đơn hàng giả lập với nhiều trạng thái khác nhau.</summary>
        Task<int> GenerateOrdersAsync(int count);
    }
}
