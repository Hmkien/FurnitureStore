using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.ViewModels;

namespace FurnitureStore.API.Interface
{
    /// <summary>Sổ địa chỉ giao hàng của người dùng (1-n, có địa chỉ mặc định).</summary>
    public interface IAddressService
    {
        Task<List<AddressVM>> GetMyAddressesAsync();
        Task<AddressVM> CreateAsync(AddressForm form);
        Task<bool> UpdateAsync(Guid id, AddressForm form);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> SetDefaultAsync(Guid id);
    }
}
