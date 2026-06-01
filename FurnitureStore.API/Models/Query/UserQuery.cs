using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.Query
{
    /// <summary>
    /// Query parameters cho User với các filter bổ sung
    /// </summary>
    public class UserQuery : BaseQuery
    {
        /// <summary>
        /// Lọc theo trạng thái người dùng
        /// </summary>
        public StatusEntity? Status { get; set; }

    }
}

