using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Repositories;
using FurnitureStore.API.Services.Base;

namespace FurnitureStore.API.Services
{
    public class RoleService : CrudService<Role, RoleForm>, IRoleService
    {
        public RoleService(IRepository<Role> repository) : base(repository)
        {
        }

        protected override Role MapToNew(RoleForm form) => new()
        {
            Name = form.Name,
            RoleCode = form.RoleCode
        };

        protected override void ApplyUpdate(Role entity, RoleForm form)
        {
            entity.Name = form.Name;
            entity.RoleCode = form.RoleCode;
        }
    }
}
