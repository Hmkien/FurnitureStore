using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.Query;
using FurnitureStore.API.Models.ViewModels;

namespace FurnitureStore.API.Interface
{
    public interface IUserService
    {
        Task<User> GetUserById(Guid id);

        Task<CurrentUserVM?> GetUserVMByIdAsync(Guid id);
        Task<CurrentUserVM?> GetUserDtoByIdAsync(Guid id);
        Task<User?> GetUserByIdAsync(Guid id, bool includeRefreshTokens = true, bool includeRoles = true);

        Task<User?> GetUserByEmailAsync(string email);

        Task<DataTableJson> GetPaged(UserQuery query);
        Task<bool> CreateUserAsync(UserForm user);

        Task<bool> Update(UserUpdateForm userForm, Guid id);

        Task<bool> DeleteUserAsync(Guid id);

        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);

        bool CheckPassword(User user, string password);

        Task<bool> Reject(Guid userId);

        Task<bool> Approved(Guid userId);

        Task<CurrentUserVM> GetCurrentUser();

        Task<bool> ChangePasswordByAdmin(Guid userId, ChangePasswordAdminRequestVM requestVM);

    }
}

