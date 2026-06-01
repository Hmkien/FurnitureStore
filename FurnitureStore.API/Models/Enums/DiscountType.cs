using System.ComponentModel;

namespace FurnitureStore.API.Models.Enums
{
    public enum DiscountType
    {
        [Description("Theo phần trăm")]
        Percentage = 1,
        [Description("Số tiền cố định")]
        Fixed = 2
    }
}
