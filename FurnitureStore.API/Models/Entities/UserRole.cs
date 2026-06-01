namespace FurnitureStore.API.Models.Entities
{
    public class UserRole
    {
        public Guid RoleId { get; set; }
        public virtual Role? Role { get; set; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
}

