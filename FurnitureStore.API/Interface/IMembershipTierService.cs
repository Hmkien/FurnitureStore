using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.ViewModels;
using FurnitureStore.API.Services.Base;

namespace FurnitureStore.API.Interface
{
    public interface IMembershipTierService : ICrudService<MembershipTier, MembershipTierForm>
    {
        /// <summary>Hạng hiện tại của user đang đăng nhập, tính theo tổng chi tiêu.</summary>
        Task<MyMembershipVM> GetMyMembershipAsync();
    }
}
