using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.ViewModels
{
    public class StockMovementVM
    {
        public Guid Id { get; set; }
        public Guid VariantId { get; set; }
        public string SkuVariant { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public StockMovementType Type { get; set; }
        public int Quantity { get; set; }
        public int QuantityAfter { get; set; }
        public string? Note { get; set; }
        public DateTime Created { get; set; }
    }
}
