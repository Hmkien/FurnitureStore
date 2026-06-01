using FurnitureStore.API.Common.Exceptions;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Enums;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.ViewModels;
using FurnitureStore.API.Repositories;
using FurnitureStore.API.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Services
{
    public class CouponService : CrudService<Coupon, CouponForm>, ICouponService
    {
        public CouponService(IRepository<Coupon> repository) : base(repository)
        {
        }

        public async Task<List<CouponPublicVM>> GetActiveAsync()
        {
            var now = DateTime.Now;
            return await Repository.Query()
                .Where(c => c.Status == StatusEntity.Approved
                    && (c.StartDate == null || c.StartDate <= now)
                    && (c.EndDate == null || c.EndDate >= now)
                    && (c.UsageLimit == 0 || c.UsedCount < c.UsageLimit))
                .OrderBy(c => c.EndDate == null)
                .ThenBy(c => c.EndDate)
                .Select(c => new CouponPublicVM
                {
                    Code = c.Code,
                    Description = c.Description,
                    DiscountType = c.DiscountType,
                    DiscountValue = c.DiscountValue,
                    MinOrderAmount = c.MinOrderAmount,
                    MaxDiscount = c.MaxDiscount,
                    EndDate = c.EndDate
                })
                .Take(12)
                .ToListAsync();
        }

        public async Task<CouponPreviewVM> PreviewAsync(string code, decimal subTotal)
        {
            var (_, discount) = await ValidateAndComputeAsync(code, subTotal);
            return new CouponPreviewVM
            {
                Code = code,
                SubTotal = subTotal,
                DiscountAmount = discount,
                AmountAfterDiscount = subTotal - discount
            };
        }

        public async Task<(Coupon coupon, decimal discount)> ValidateAndComputeAsync(string code, decimal subTotal)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new BadRequestException("Mã giảm giá không hợp lệ");

            var coupon = await Repository.Query().FirstOrDefaultAsync(c => c.Code == code)
                ?? throw new BadRequestException("Mã giảm giá không tồn tại");

            var now = DateTime.Now;
            if (coupon.Status != StatusEntity.Approved)
                throw new BadRequestException("Mã giảm giá đã ngừng áp dụng");
            if (coupon.StartDate.HasValue && now < coupon.StartDate.Value)
                throw new BadRequestException("Mã giảm giá chưa tới thời gian áp dụng");
            if (coupon.EndDate.HasValue && now > coupon.EndDate.Value)
                throw new BadRequestException("Mã giảm giá đã hết hạn");
            if (coupon.UsageLimit > 0 && coupon.UsedCount >= coupon.UsageLimit)
                throw new BadRequestException("Mã giảm giá đã hết lượt sử dụng");
            if (subTotal < coupon.MinOrderAmount)
                throw new BadRequestException($"Đơn tối thiểu {coupon.MinOrderAmount:#,##0} để dùng mã này");

            var discount = coupon.DiscountType == DiscountType.Percentage
                ? subTotal * coupon.DiscountValue / 100m
                : coupon.DiscountValue;

            if (coupon.MaxDiscount > 0 && discount > coupon.MaxDiscount)
                discount = coupon.MaxDiscount;
            if (discount > subTotal)
                discount = subTotal;

            return (coupon, decimal.Round(discount, 2));
        }

        protected override async Task ValidateAsync(CouponForm form, Guid? existingId = null)
        {
            if (await Repository.AnyAsync(c => c.Code == form.Code && c.Id != existingId))
                throw new ConflictException($"Mã '{form.Code}' đã tồn tại");
        }

        protected override Coupon MapToNew(CouponForm form) => new()
        {
            Code = form.Code,
            Description = form.Description,
            DiscountType = form.DiscountType,
            DiscountValue = form.DiscountValue,
            MinOrderAmount = form.MinOrderAmount,
            MaxDiscount = form.MaxDiscount,
            StartDate = form.StartDate,
            EndDate = form.EndDate,
            UsageLimit = form.UsageLimit
        };

        protected override void ApplyUpdate(Coupon entity, CouponForm form)
        {
            entity.Code = form.Code;
            entity.Description = form.Description;
            entity.DiscountType = form.DiscountType;
            entity.DiscountValue = form.DiscountValue;
            entity.MinOrderAmount = form.MinOrderAmount;
            entity.MaxDiscount = form.MaxDiscount;
            entity.StartDate = form.StartDate;
            entity.EndDate = form.EndDate;
            entity.UsageLimit = form.UsageLimit;
        }
    }
}
