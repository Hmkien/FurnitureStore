using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.Query;
using FurnitureStore.API.Models.ViewModels;

namespace FurnitureStore.API.Interface
{
    public interface IRoleService
    {
        Task<Role> Create(RoleForm role);
        Task<Role> GetByIdAsync(Guid id);
        Task<bool> Update(Guid id, RoleForm form);
        Task<DataTableJson> GetPaged(BaseQuery query);
        Task<bool> Approved(Guid id);
        Task<bool> Reject(Guid id);
    }
}

