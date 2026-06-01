using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.Query
{
    /// <summary>
    /// Query cho sản phẩm với bộ lọc chuyên sâu (danh mục, khoảng giá, chất liệu, phong cách, tình trạng).
    /// </summary>
    public class ProductQuery : BaseQuery
    {
        public Guid? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Material { get; set; }
        public string? Style { get; set; }
        public ProductCondition? Condition { get; set; }
    }
}
