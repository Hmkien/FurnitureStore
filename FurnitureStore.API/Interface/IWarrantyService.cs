using FurnitureStore.API.Models.Enums;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.Query;
using FurnitureStore.API.Models.ViewModels;

namespace FurnitureStore.API.Interface
{
    public interface IWarrantyService
    {
        Task<ReturnRequestResult> CreateAsync(CreateWarrantyForm form);
        Task<DataTableJson> GetAllAsync(BaseQuery query);
        Task<DataTableJson> GetMyAsync(BaseQuery query);
        Task UpdateStatusAsync(Guid id, WarrantyStatus status, string? note);
    }
}
