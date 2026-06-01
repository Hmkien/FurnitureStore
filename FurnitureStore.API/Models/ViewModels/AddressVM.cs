namespace FurnitureStore.API.Models.ViewModels
{
    public class AddressVM
    {
        public Guid Id { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public string? Label { get; set; }
        public bool IsDefault { get; set; }
    }
}
