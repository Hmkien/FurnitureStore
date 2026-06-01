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
using Microsoft.Extensions.Caching.Memory;

namespace FurnitureStore.API.Services
{
    public class ReviewService : CrudService<Review, ReviewForm>, IReviewService
    {
        private readonly ApplicationDbContext _context;
        private readonly RequestContext _requestContext;
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan OtpTtl = TimeSpan.FromMinutes(5);

        public ReviewService(
            IRepository<Review> repository,
            ApplicationDbContext context,
            RequestContext requestContext,
            IMemoryCache cache) : base(repository)
        {
            _context = context;
            _requestContext = requestContext;
            _cache = cache;
        }

        private string OtpKey(Guid userId, Guid productId) => $"review-otp:{userId}:{productId}";

        public Task<string> RequestOtpAsync(Guid productId)
        {
            var userId = _requestContext.GetUserId();
            var otp = Random.Shared.Next(100000, 1000000).ToString();
            _cache.Set(OtpKey(userId, productId), otp, OtpTtl);
            // Chưa tích hợp email/SMS: trả OTP về cho client (môi trường demo).
            return Task.FromResult(otp);
        }

        // Đánh giá hiển thị ngay (do khách tạo); admin Reject để ẩn. Không khóa sửa.
        protected override StatusEntity InitialStatus => StatusEntity.Approved;
        protected override bool LockWhenApproved => false;

        public async Task<ProductReviewSummaryVM> GetByProductAsync(Guid productId)
        {
            var reviews = await Repository.Query()
                .Where(r => r.ProductId == productId && r.Status == StatusEntity.Approved)
                .OrderByDescending(r => r.Created)
                .Select(r => new ReviewVM
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    ReviewerName = r.User!.UserName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ImageUrl = r.ImageUrl,
                    Created = r.Created
                })
                .ToListAsync();

            return new ProductReviewSummaryVM
            {
                ProductId = productId,
                TotalReviews = reviews.Count,
                AverageRating = reviews.Count > 0 ? Math.Round(reviews.Average(r => r.Rating), 1) : 0,
                Reviews = reviews
            };
        }

        protected override async Task ValidateAsync(ReviewForm form, Guid? existingId = null)
        {
            var userId = _requestContext.GetUserId();

            if (existingId == null)
            {
                var purchased = await _context.OrderItems.AnyAsync(oi =>
                    oi.Order!.UserId == userId
                    && oi.Order.OrderStatus == OrderStatus.Completed
                    && oi.Variant!.ProductId == form.ProductId);

                if (!purchased)
                    throw new BadRequestException("Bạn chỉ có thể đánh giá sản phẩm đã mua và nhận hàng");

                if (await Repository.AnyAsync(r => r.ProductId == form.ProductId && r.UserId == userId))
                    throw new ConflictException("Bạn đã đánh giá sản phẩm này rồi");

                var key = OtpKey(userId, form.ProductId);
                if (!_cache.TryGetValue(key, out string? otp) || otp != form.Otp)
                    throw new BadRequestException("Mã OTP không đúng hoặc đã hết hạn");
                _cache.Remove(key);
            }
            else
            {
                // Chỉ chủ đánh giá mới được sửa.
                var ownerId = await Repository.Query()
                    .Where(r => r.Id == existingId)
                    .Select(r => r.UserId)
                    .FirstOrDefaultAsync();

                if (ownerId != userId && !_requestContext.IsSuperUser())
                    throw new UnauthorizedAccessException("Bạn không có quyền sửa đánh giá này");
            }
        }

        protected override Review MapToNew(ReviewForm form) => new()
        {
            ProductId = form.ProductId,
            UserId = _requestContext.GetUserId(),
            Rating = form.Rating,
            Comment = form.Comment,
            ImageUrl = form.ImageUrl
        };

        protected override void ApplyUpdate(Review entity, ReviewForm form)
        {
            entity.Rating = form.Rating;
            entity.Comment = form.Comment;
            entity.ImageUrl = form.ImageUrl;
        }
    }
}
