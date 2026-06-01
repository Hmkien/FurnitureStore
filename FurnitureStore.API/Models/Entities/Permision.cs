namespace FurnitureStore.API.Models.Entities
{
    public class Permision : BaseEntity
    {
        public string PermisionName { get; set; } = string.Empty;
        public string PermisionCode { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}

