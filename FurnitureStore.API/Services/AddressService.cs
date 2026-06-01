using FurnitureStore.API.Common.Exceptions;
using FurnitureStore.API.Data;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Middlewares;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Services
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext _context;
        private readonly RequestContext _requestContext;

        public AddressService(ApplicationDbContext context, RequestContext requestContext)
        {
            _context = context;
            _requestContext = requestContext;
        }

        public Task<List<AddressVM>> GetMyAddressesAsync()
        {
            var userId = _requestContext.GetUserId();
            return _context.UserAddresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.Created)
                .Select(a => new AddressVM
                {
                    Id = a.Id,
                    ReceiverName = a.ReceiverName,
                    ReceiverPhone = a.ReceiverPhone,
                    AddressLine = a.AddressLine,
                    Label = a.Label,
                    IsDefault = a.IsDefault
                })
                .ToListAsync();
        }

        public async Task<AddressVM> CreateAsync(AddressForm form)
        {
            var userId = _requestContext.GetUserId();
            var isFirst = !await _context.UserAddresses.AnyAsync(a => a.UserId == userId);

            var entity = new UserAddress
            {
                UserId = userId,
                ReceiverName = form.ReceiverName,
                ReceiverPhone = form.ReceiverPhone,
                AddressLine = form.AddressLine,
                Label = form.Label,
                IsDefault = form.IsDefault || isFirst
            };

            if (entity.IsDefault)
                await ClearDefaultAsync(userId);

            _context.UserAddresses.Add(entity);
            await _context.SaveChangesAsync();
            return ToVm(entity);
        }

        public async Task<bool> UpdateAsync(Guid id, AddressForm form)
        {
            var entity = await GetOwnedAsync(id);
            entity.ReceiverName = form.ReceiverName;
            entity.ReceiverPhone = form.ReceiverPhone;
            entity.AddressLine = form.AddressLine;
            entity.Label = form.Label;

            if (form.IsDefault && !entity.IsDefault)
            {
                await ClearDefaultAsync(entity.UserId);
                entity.IsDefault = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetOwnedAsync(id);
            var wasDefault = entity.IsDefault;
            _context.UserAddresses.Remove(entity);
            await _context.SaveChangesAsync();

            // Nếu xóa địa chỉ mặc định, đặt địa chỉ còn lại mới nhất làm mặc định.
            if (wasDefault)
            {
                var next = await _context.UserAddresses
                    .Where(a => a.UserId == entity.UserId)
                    .OrderByDescending(a => a.Created)
                    .FirstOrDefaultAsync();
                if (next != null)
                {
                    next.IsDefault = true;
                    await _context.SaveChangesAsync();
                }
            }
            return true;
        }

        public async Task<bool> SetDefaultAsync(Guid id)
        {
            var entity = await GetOwnedAsync(id);
            await ClearDefaultAsync(entity.UserId);
            entity.IsDefault = true;
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<UserAddress> GetOwnedAsync(Guid id)
        {
            var userId = _requestContext.GetUserId();
            return await _context.UserAddresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId)
                ?? throw new NotFoundException("Không tìm thấy địa chỉ");
        }

        private async Task ClearDefaultAsync(Guid userId)
        {
            var defaults = await _context.UserAddresses
                .Where(a => a.UserId == userId && a.IsDefault)
                .ToListAsync();
            foreach (var d in defaults) d.IsDefault = false;
        }

        private static AddressVM ToVm(UserAddress a) => new()
        {
            Id = a.Id,
            ReceiverName = a.ReceiverName,
            ReceiverPhone = a.ReceiverPhone,
            AddressLine = a.AddressLine,
            Label = a.Label,
            IsDefault = a.IsDefault
        };
    }
}
