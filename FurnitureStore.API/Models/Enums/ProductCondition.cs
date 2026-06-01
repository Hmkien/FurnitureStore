using System.ComponentModel;

namespace FurnitureStore.API.Models.Enums
{
    /// <summary>
    /// Tình trạng hàng của một biến thể (cho phép bán hàng trả lại / like-new).
    /// </summary>
    public enum ProductCondition
    {
        [Description("Mới 100%")]
        New = 1,
        [Description("Như mới 99%")]
        LikeNew99 = 2,
        [Description("Like new")]
        LikeNew = 3,
        [Description("Hàng tân trang")]
        Refurbished = 4,
        [Description("Hàng trả lại")]
        Returned = 5
    }
}
