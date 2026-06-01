namespace FurnitureStore.API.Models.ViewModels
{
    public class CartVM
    {
        public Guid Id { get; set; }
        public List<CartItemVM> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public decimal SubTotal { get; set; }
    }

    public class CartItemVM
    {
        public Guid Id { get; set; }
        public Guid VariantId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Size { get; set; }
        public string? Material { get; set; }
        public string? Color { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public int StockQuantity { get; set; }
    }
}
