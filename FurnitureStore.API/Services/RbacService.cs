using System.Reflection;
using FurnitureStore.API.Common.Exceptions;
using FurnitureStore.API.Data;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Enums;
using FurnitureStore.API.Models.ViewModels;
using FurnitureStore.API.Services.Seed;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Services
{
    public class RbacService : IRbacService
    {
        private readonly ApplicationDbContext _context;

        public RbacService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===== Metadata từ enum (code -> tên hiển thị, module) =====
        private static readonly Dictionary<string, (string Name, string Module)> Meta = BuildMeta();

        private static Dictionary<string, (string, string)> BuildMeta()
        {
            var map = new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase);
            foreach (var field in typeof(PermissionCode).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attr = field.GetCustomAttribute<PermissionMetaAttribute>();
                map[field.Name] = (attr?.Name ?? field.Name, attr?.Module ?? "Khác");
            }
            return map;
        }

        // ===== Đồng bộ RBAC từ enum =====
        public async Task<RbacSyncResultVM> SyncAsync()
        {
            // 1) Quyền
            var permByCode = await _context.Permisions
                .ToDictionaryAsync(p => p.PermisionCode, StringComparer.OrdinalIgnoreCase);
            var permCreated = 0;

            foreach (var pc in Enum.GetValues<PermissionCode>())
            {
                var code = pc.ToString();
                var (name, _) = Meta[code];
                if (permByCode.TryGetValue(code, out var existing))
                {
                    existing.PermisionName = name;
                    existing.Status = StatusEntity.Approved;
                }
                else
                {
                    var p = new Permision
                    {
                        PermisionCode = code,
                        PermisionName = name,
                        Status = StatusEntity.Approved
                    };
                    _context.Permisions.Add(p);
                    permByCode[code] = p;
                    permCreated++;
                }
            }
            await _context.SaveChangesAsync();

            // 2) Vai trò
            var roleByCode = await _context.Roles
                .ToDictionaryAsync(r => r.RoleCode, StringComparer.OrdinalIgnoreCase);
            var roleCreated = 0;

            foreach (var dr in Enum.GetValues<DefaultRole>())
            {
                var code = dr.ToString();
                if (roleByCode.TryGetValue(code, out var existing))
                {
                    existing.Name = RbacSeedData.RoleNames[dr];
                    existing.Status = StatusEntity.Approved;
                }
                else
                {
                    var r = new Role
                    {
                        RoleCode = code,
                        Name = RbacSeedData.RoleNames[dr],
                        Status = StatusEntity.Approved
                    };
                    _context.Roles.Add(r);
                    roleByCode[code] = r;
                    roleCreated++;
                }
            }
            await _context.SaveChangesAsync();

            // 3) Map quyền cho vai trò (chỉ thêm quyền còn thiếu, không xóa cấu hình thủ công)
            var mappingsCreated = 0;
            foreach (var dr in Enum.GetValues<DefaultRole>())
            {
                var role = roleByCode[dr.ToString()];
                var wantedCodes = dr == DefaultRole.ADMIN
                    ? Enum.GetValues<PermissionCode>()
                    : RbacSeedData.RolePermissions[dr];

                var wantedIds = wantedCodes
                    .Select(c => permByCode[c.ToString()].Id)
                    .ToHashSet();

                var currentIds = await _context.RolePermisions
                    .Where(rp => rp.RoleId == role.Id)
                    .Select(rp => rp.PermissionId)
                    .ToListAsync();

                foreach (var pid in wantedIds.Except(currentIds))
                {
                    _context.RolePermisions.Add(new RolePermision
                    {
                        RoleId = role.Id,
                        PermissionId = pid,
                        Created = DateTime.Now
                    });
                    mappingsCreated++;
                }
            }
            await _context.SaveChangesAsync();

            return new RbacSyncResultVM
            {
                PermissionsCreated = permCreated,
                RolesCreated = roleCreated,
                MappingsCreated = mappingsCreated,
                Message = $"Đồng bộ xong: +{permCreated} quyền, +{roleCreated} vai trò, +{mappingsCreated} liên kết quyền."
            };
        }

        // ===== Danh sách phục vụ UI =====
        public async Task<List<PermissionOptionVM>> GetAllPermissionsAsync()
        {
            var perms = await _context.Permisions
                .OrderBy(p => p.PermisionCode)
                .Select(p => new { p.Id, p.PermisionCode, p.PermisionName })
                .ToListAsync();

            return perms.Select(p => new PermissionOptionVM
            {
                Id = p.Id,
                Code = p.PermisionCode,
                Name = p.PermisionName,
                Module = Meta.TryGetValue(p.PermisionCode, out var m) ? m.Module : "Khác"
            }).ToList();
        }

        public Task<List<RoleOptionVM>> GetAllRolesAsync()
            => _context.Roles
                .OrderBy(r => r.Name)
                .Select(r => new RoleOptionVM { Id = r.Id, Name = r.Name, RoleCode = r.RoleCode })
                .ToListAsync();

        // ===== Phân quyền cho vai trò =====
        public Task<List<Guid>> GetRolePermissionIdsAsync(Guid roleId)
            => _context.RolePermisions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

        public async Task SetRolePermissionsAsync(Guid roleId, List<Guid> permissionIds)
        {
            if (!await _context.Roles.AnyAsync(r => r.Id == roleId))
                throw new NotFoundException("Không tìm thấy vai trò");

            var distinctIds = permissionIds.Distinct().ToList();
            var validIds = await _context.Permisions
                .Where(p => distinctIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            var current = await _context.RolePermisions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            _context.RolePermisions.RemoveRange(current);
            foreach (var pid in validIds)
            {
                _context.RolePermisions.Add(new RolePermision
                {
                    RoleId = roleId,
                    PermissionId = pid,
                    Created = DateTime.Now
                });
            }
            await _context.SaveChangesAsync();
        }

        // ===== Phân vai trò cho người dùng =====
        public Task<List<Guid>> GetUserRoleIdsAsync(Guid userId)
            => _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

        public async Task SetUserRolesAsync(Guid userId, List<Guid> roleIds)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == userId))
                throw new NotFoundException("Không tìm thấy người dùng");

            var distinctIds = roleIds.Distinct().ToList();
            var validIds = await _context.Roles
                .Where(r => distinctIds.Contains(r.Id))
                .Select(r => r.Id)
                .ToListAsync();

            var current = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            _context.UserRoles.RemoveRange(current);
            foreach (var rid in validIds)
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = userId,
                    RoleId = rid,
                    Created = DateTime.Now
                });
            }
            await _context.SaveChangesAsync();
        }
    }
}
