using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.ViewModels
{
    public class ReturnItemVM
    {
        public Guid VariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public class ReturnVM
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string OrderCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public ReturnType Type { get; set; }
        public ReturnStatus RequestStatus { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime Created { get; set; }
        public List<ReturnItemVM> Items { get; set; } = new();
    }

    public class WarrantyVM
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string OrderCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string IssueDescription { get; set; } = string.Empty;
        public WarrantyStatus ClaimStatus { get; set; }
        public string? Note { get; set; }
        public DateTime Created { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
