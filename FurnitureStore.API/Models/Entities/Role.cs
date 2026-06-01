namespace FurnitureStore.API.Models.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public virtual ICollection<UserRole>? UserRoles { get; set; }
        public virtual ICollection<RolePermision>? RolePermisions { get; set; }

    }
}

