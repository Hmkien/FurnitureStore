using System.ComponentModel;

namespace FurnitureStore.API.Models.Enums
{
    /// <summary>Loại yêu cầu hậu mãi.</summary>
    public enum ReturnType
    {
        [Description("Đổi hàng")]
        Exchange = 1,
        [Description("Trả hàng")]
        Return = 2,
        [Description("Hoàn tiền")]
        Refund = 3
    }

    /// <summary>Trạng thái xử lý yêu cầu đổi/trả.</summary>
    public enum ReturnStatus
    {
        [Description("Chờ xử lý")]
        Requested = 1,
        [Description("Đã duyệt")]
        Approved = 2,
        [Description("Từ chối")]
        Rejected = 3,
        [Description("Hoàn tất")]
        Completed = 4
    }

    /// <summary>Trạng thái phiếu bảo hành.</summary>
    public enum WarrantyStatus
    {
        [Description("Tiếp nhận")]
        Received = 1,
        [Description("Đang xử lý")]
        Processing = 2,
        [Description("Hoàn tất")]
        Completed = 3,
        [Description("Từ chối")]
        Rejected = 4
    }
}
