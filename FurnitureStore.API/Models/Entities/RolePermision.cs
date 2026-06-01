namespace FurnitureStore.API.Models.Entities
{
    public class RolePermision
    {
        public Guid RoleId { get; set; }
        public virtual Role? Role { get; set; }
        public Guid PermissionId { get; set; }
        public virtual Permision? Permision { get; set; }
        public DateTime Created { get; set; }
    }
}

