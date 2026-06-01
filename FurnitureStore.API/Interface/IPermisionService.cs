using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.Query;
using FurnitureStore.API.Models.ViewModels;

namespace FurnitureStore.API.Interface
{
    public interface IPermisionService
    {
        Task<Permision> Create(PermisionForm permision);
        Task<Permision> GetByIdAsync(Guid id);
        Task<bool> Update(Guid id, PermisionForm form);
        Task<DataTableJson> GetPaged(BaseQuery query);
        Task<bool> Approved(Guid id);
        Task<bool> Reject(Guid id);
    }
}

