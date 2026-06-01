using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.Query
{
    /// <summary>Query đơn hàng với lọc theo trạng thái.</summary>
    public class OrderQuery : BaseQuery
    {
        public OrderStatus? OrderStatus { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
    }
}
