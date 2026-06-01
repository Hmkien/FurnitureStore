namespace FurnitureStore.API.Models.ViewModels
{
    /// <summary>Các giá trị lọc khả dụng để đổ vào bộ lọc storefront.</summary>
    public class ProductFilterOptionsVM
    {
        public List<string> Materials { get; set; } = new();
        public List<string> Colors { get; set; } = new();
        public List<string> Styles { get; set; } = new();
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
