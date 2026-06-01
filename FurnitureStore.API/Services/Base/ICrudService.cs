using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Query;
using FurnitureStore.API.Models.ViewModels;

namespace FurnitureStore.API.Services.Base
{
    /// <summary>
    /// Hợp đồng CRUD chung cho các entity có vòng đời Approve/Reject.
    /// </summary>
    public interface ICrudService<TEntity, TForm> where TEntity : BaseEntity
    {
        Task<TEntity> GetByIdAsync(Guid id);
        Task<TEntity> Create(TForm form);
        Task<bool> Update(Guid id, TForm form);
        Task<bool> Delete(Guid id);
        Task<bool> Approved(Guid id);
        Task<bool> Reject(Guid id);
        Task<DataTableJson> GetPaged(BaseQuery query);
    }
}
