namespace FurnitureStore.API.Models.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool IsSuperUser { get; set; }
        public string? ImageAvatar { get; set; }
        public virtual ICollection<UserRole>? UserRoles { get; set; }
    }
}

