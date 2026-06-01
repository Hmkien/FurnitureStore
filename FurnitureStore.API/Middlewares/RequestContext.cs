using FurnitureStore.API.Models.ViewModels;

namespace FurnitureStore.API.Middlewares
{
    /// <summary>
    /// RequestContext lưu thông tin người dùng hiện tại trong scope của request
    /// </summary>
    public class RequestContext
    {
        /// <summary>
        /// Thông tin người dùng hiện tại
        /// </summary>
        public CurrentUserVM? CurrentUser { get; set; }

        /// <summary>
        /// Kiểm tra user đã đăng nhập hay chưa
        /// </summary>
        public bool IsAuthenticated => CurrentUser != null;

        /// <summary>
        /// Lấy UserId (throw exception nếu chưa đăng nhập)
        /// </summary>
        public Guid GetUserId()
        {
            if (CurrentUser == null)
                throw new UnauthorizedAccessException("User chưa đăng nhập");

            return CurrentUser.UserId;
        }

        /// <summary>
        /// Lấy UserName (throw exception nếu chưa đăng nhập)
        /// </summary>
        public string GetUserName()
        {
            if (CurrentUser == null)
                throw new UnauthorizedAccessException("User chưa đăng nhập");

            return CurrentUser.UserName;
        }

        /// <summary>
        /// Kiểm tra user có quyền hay không
        /// </summary>
        public bool HasPermission(string permission)
        {
            if (CurrentUser == null) return false;
            if (CurrentUser.IsSuperUser) return true;
            return CurrentUser.Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Kiểm tra user có role hay không
        /// </summary>
        public bool HasRole(string role)
        {
            if (CurrentUser == null) return false;
            if (CurrentUser.IsSuperUser) return true;
            return CurrentUser.Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Kiểm tra user có bất kỳ quyền nào trong danh sách hay không
        /// </summary>
        public bool HasAnyPermission(params string[] permissions)
        {
            if (CurrentUser == null) return false;
            if (CurrentUser.IsSuperUser) return true;
            return permissions.Any(p => CurrentUser.Permissions.Contains(p, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Kiểm tra user có tất cả quyền trong danh sách hay không
        /// </summary>
        public bool HasAllPermissions(params string[] permissions)
        {
            if (CurrentUser == null) return false;
            if (CurrentUser.IsSuperUser) return true;
            return permissions.All(p => CurrentUser.Permissions.Contains(p, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Kiểm tra user có phải SuperUser hay không
        /// </summary>
        public bool IsSuperUser()
        {
            return CurrentUser?.IsSuperUser ?? false;
        }
    }
}

