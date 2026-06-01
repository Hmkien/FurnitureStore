using System.ComponentModel;

namespace FurnitureStore.API.Models.Enums
{
    /// <summary>Loại biến động kho.</summary>
    public enum StockMovementType
    {
        [Description("Nhập kho")]
        Import = 1,
        [Description("Xuất kho")]
        Export = 2,
        [Description("Điều chỉnh")]
        Adjust = 3
    }
}
