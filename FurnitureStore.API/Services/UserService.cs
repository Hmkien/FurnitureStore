using FurnitureStore.API.Common.Exceptions;
using FurnitureStore.API.Data;
using FurnitureStore.API.Extensions;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Middlewares;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.Query;
using FurnitureStore.API.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly RequestContext _requestContext;
        private readonly IWebHostEnvironment _env;

        public UserService(ApplicationDbContext context, RequestContext requestContext, IWebHostEnvironment env)
        {
            _context = context;
            _requestContext = requestContext;
            _env = env;
        }

        #region User Management

        public async Task<bool> CreateUserAsync(UserForm userForm)
        {
            if (await _context.Users.AnyAsync(u => u.Email == userForm.Email || u.UserName == userForm.UserName))
                throw new ConflictException("Thông tin người dùng đã tồn tại!");
            var user = MapUserFormToEntity(userForm);
            if (userForm.ImageAvatar != null)
            {
                var path = await SaveAvatarAsync(userForm.ImageAvatar);
                user.ImageAvatar = path;
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Update(UserUpdateForm userVM, Guid id)
        {
            var user = await GetUserEntityById(id);
            if (user == null) return false;
            if (userVM.ImageAvatar != null)
            {
                if (!string.IsNullOrEmpty(user.ImageAvatar))
                {
                    try
                    {
                        var oldPath = Path.Combine(_env.WebRootPath, user.ImageAvatar.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (File.Exists(oldPath)) File.Delete(oldPath);
                    }
                    catch
                    {
                    }
                }
                user.ImageAvatar = await SaveAvatarAsync(userVM.ImageAvatar);
            }
            else if (userVM.RemoveImage)
            {
                if (!string.IsNullOrEmpty(user.ImageAvatar))
                {
                    try
                    {
                        var oldPath = Path.Combine(_env.WebRootPath, user.ImageAvatar.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (File.Exists(oldPath)) File.Delete(oldPath);
                    }
                    catch { }
                }
                user.ImageAvatar = null;
            }

            UpdateUserEntity(user, userVM);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await GetUserEntityById(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region User Status Management

        public async Task<bool> Approved(Guid userId)
        {
            return await UpdateUserStatus(userId, Models.Enums.StatusEntity.Approved);
        }

        public async Task<bool> Reject(Guid userId)
        {
            return await UpdateUserStatus(userId, Models.Enums.StatusEntity.Rejected);
        }

        #endregion

        #region Password Management

        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await GetUserEntityById(userId);
            if (user == null || !CheckPassword(user, currentPassword))
                return false;

            user.PasswordHash = HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordByAdmin(Guid userId, ChangePasswordAdminRequestVM requestVM)
        {
            var user = await GetUserEntityById(userId);
            if (user == null) return false;

            user.PasswordHash = HashPassword(requestVM.NewPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool CheckPassword(User user, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        #endregion

        #region Get User Data

        /// <summary>
        /// Lấy thông tin user hiện tại từ RequestContext
        /// </summary>
        public async Task<CurrentUserVM> GetCurrentUser()
        {
            if (!_requestContext.IsAuthenticated)
                throw new UnauthorizedAccessException("User chưa đăng nhập");
            return _requestContext.CurrentUser!;
        }

        public async Task<CurrentUserVM?> GetUserDtoByIdAsync(Guid id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Include(u => u.UserRoles!)
                    .ThenInclude(ur => ur.Role!)
                        .ThenInclude(r => r.RolePermisions!)
                            .ThenInclude(rp => rp.Permision)
                .Select(u => new CurrentUserVM
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Token = string.Empty,
                    FullName = $"{u.LastName} {u.FirstName}" ?? string.Empty,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber ?? string.Empty,
                    Address = u.Address,
                    IsSuperUser = u.IsSuperUser,
                    BirthDay = u.Birthday,
                    Roles = u.UserRoles!
                        .Where(ur => ur.Role != null)
                        .Select(ur => ur.Role!.RoleCode)
                        .Distinct()
                        .ToList(),
                    Permissions = u.UserRoles!
                        .Where(ur => ur.Role != null)
                        .SelectMany(ur => ur.Role!.RolePermisions!
                            .Where(rp => rp.Permision != null)
                            .Select(rp => rp.Permision!.PermisionCode))
                        .Distinct()
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CurrentUserVM?> GetUserVMByIdAsync(Guid id)
        {
            var user = await GetUserEntityById(id);
            if (user == null) return null;

            return new CurrentUserVM
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Address = user.Address,
                IsSuperUser = user.IsSuperUser,
                BirthDay = user.Birthday,
                Roles = user.UserRoles?.Select(ur => ur.Role?.Name ?? "").ToList() ?? new List<string>()
            };
        }

        public async Task<User> GetUserById(Guid id)
        {
            var result = await GetUserEntityById(id);
            if (result == null) throw new NotFoundException("Bản ghi không tồn tại");
            return result;
        }

        public async Task<User?> GetUserByIdAsync(Guid id, bool includeRefreshTokens = true, bool includeRoles = true)
        {
            var query = _context.Users.AsQueryable();

            if (includeRoles)
                query = query.Include(u => u.UserRoles);

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        #endregion

        #region Paged Query - Using QueryHelper Pattern

        public async Task<DataTableJson> GetPaged(UserQuery query)
        {
            var filtered = _context.Users
                .ApplyQuery(query)
                .WithDynamicSearch()
                .WithFilterIf(query.Status.HasValue, x => x.Status == query.Status!.Value)
                .WithDateFilter(x => x.Created)
                .WithSort("Created")
                .GetQuery();

            var recordsTotal = await _context.Users.CountAsync();
            var recordsFiltered = await filtered.CountAsync();
            var data = await filtered
                .Paginate(query)
                .Select(u => new UserListItemVM
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Birthday = u.Birthday,
                    PhoneNumber = u.PhoneNumber,
                    Address = u.Address,
                    IsSuperUser = u.IsSuperUser,
                    ImageAvatar = u.ImageAvatar,
                    Status = u.Status,
                    Created = u.Created,
                    LastModified = u.LastModified
                })
                .ToListAsync();

            return new DataTableJson
            {
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        #endregion

        #region Private Helper Methods

        private async Task<User?> GetUserEntityById(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        private async Task<bool> UpdateUserStatus(Guid userId, Models.Enums.StatusEntity status)
        {
            var user = await GetUserEntityById(userId);
            if (user == null) return false;

            user.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        private User MapUserFormToEntity(UserForm userForm)
        {
            return new User
            {
                UserName = userForm.UserName,
                Email = userForm.Email,
                PasswordHash = HashPassword(userForm.Password),
                FirstName = userForm.FirstName,
                LastName = userForm.LastName,
                Birthday = userForm.Birthday,
                PhoneNumber = userForm.PhoneNumber,
                Address = userForm.Address,
                ImageAvatar = null
            };
        }

        private void UpdateUserEntity(User user, UserUpdateForm userForm)
        {
            user.Email = userForm.Email;
            user.FirstName = userForm.FirstName;
            user.LastName = userForm.LastName;
            user.Birthday = userForm.Birthday;
            user.PhoneNumber = userForm.PhoneNumber;
            user.Address = userForm.Address;
        }

        private async Task<string?> SaveAvatarAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "avatars");
            Directory.CreateDirectory(uploadsFolder);
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"/uploads/avatars/{fileName}";
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        #endregion
    }
}
