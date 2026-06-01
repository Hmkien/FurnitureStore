using FurnitureStore.API.Models.Enums;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.Query;
using FurnitureStore.API.Models.ViewModels;

namespace FurnitureStore.API.Interface
{
    public interface IReturnService
    {
        Task<ReturnRequestResult> CreateAsync(CreateReturnForm form);
        Task<DataTableJson> GetAllAsync(BaseQuery query);
        Task<DataTableJson> GetMyAsync(BaseQuery query);
        Task<ReturnVM> GetDetailAsync(Guid id);
        Task UpdateStatusAsync(Guid id, ReturnStatus status, string? note);
    }

    public record ReturnRequestResult(Guid Id, string Code);
}
