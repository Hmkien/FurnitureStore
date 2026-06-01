using System.ComponentModel;

namespace FurnitureStore.API.Models.Enums
{
    public enum StatusEntity
    {
        [Description("Đã duyệt")]
        Approved = 1,
        [Description("Chờ duyệt")]
        Pending = 2,
        [Description("Từ chối")]
        Rejected = 3,
        [Description("Bản nháp")]
        Draft = 4
    }
}

