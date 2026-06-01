using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.API.Models.Query
{
    /// <summary>
    /// Lớp truy vấn cơ bản cho phân trang, tìm kiếm, sắp xếp và lọc theo ngày.
    /// </summary>
    public class BaseQuery
    {
        /// <summary>
        /// Số lượng bản ghi trên mỗi trang (bắt buộc, lớn hơn 0).
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PageSize phải lớn hơn 0")]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Số trang hiện tại (bắt buộc, lớn hơn 0).
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber phải lớn hơn 0")]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Từ khóa tìm kiếm.
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// Tên trường cần sắp xếp.
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Sắp xếp giảm dần hay không (true: giảm dần, false: tăng dần).
        /// </summary>
        public bool SortDesc { get; set; } = false;

        /// <summary>
        /// Lọc từ ngày.
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Lọc đến ngày.
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Danh sách tên trường để search (do client quyết định).
        /// Ví dụ: ["UserName", "Email", "PhoneNumber"]
        /// </summary>
        public string[]? SearchIn { get; set; }
    }
}
