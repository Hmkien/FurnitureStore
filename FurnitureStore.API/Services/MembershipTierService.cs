using FurnitureStore.API.Common.Exceptions;
using FurnitureStore.API.Data;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Middlewares;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Enums;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.ViewModels;
using FurnitureStore.API.Repositories;
using FurnitureStore.API.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Services
{
    public class MembershipTierService : CrudService<MembershipTier, MembershipTierForm>, IMembershipTierService
    {
        private readonly ApplicationDbContext _context;
        private readonly RequestContext _requestContext;

        public MembershipTierService(
            IRepository<MembershipTier> repository,
            ApplicationDbContext context,
            RequestContext requestContext) : base(repository)
        {
            _context = context;
            _requestContext = requestContext;
        }

        public async Task<MyMembershipVM> GetMyMembershipAsync()
        {
            var userId = _requestContext.GetUserId();

            var totalSpending = await _context.Orders
                .Where(o => o.UserId == userId && o.OrderStatus == OrderStatus.Completed)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            var tiers = await Repository.Query()
                .OrderBy(t => t.MinSpending)
                .ToListAsync();

            var current = tiers.LastOrDefault(t => totalSpending >= t.MinSpending);
            var next = tiers.FirstOrDefault(t => t.MinSpending > totalSpending);

            return new MyMembershipVM
            {
                TotalSpending = totalSpending,
                CurrentTier = current?.Name,
                CurrentDiscountPercent = current?.DiscountPercent ?? 0,
                NextTier = next?.Name,
                AmountToNextTier = next != null ? next.MinSpending - totalSpending : null
            };
        }

        protected override async Task ValidateAsync(MembershipTierForm form, Guid? existingId = null)
        {
            if (await Repository.AnyAsync(t => t.Name == form.Name && t.Id != existingId))
                throw new ConflictException($"Hạng '{form.Name}' đã tồn tại");
        }

        protected override MembershipTier MapToNew(MembershipTierForm form) => new()
        {
            Name = form.Name,
            MinSpending = form.MinSpending,
            DiscountPercent = form.DiscountPercent,
            Description = form.Description
        };

        protected override void ApplyUpdate(MembershipTier entity, MembershipTierForm form)
        {
            entity.Name = form.Name;
            entity.MinSpending = form.MinSpending;
            entity.DiscountPercent = form.DiscountPercent;
            entity.Description = form.Description;
        }
    }
}
