using FurnitureStore.API.Common.Exceptions;
using FurnitureStore.API.Data;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Middlewares;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly ApplicationDbContext _context;
        private readonly RequestContext _requestContext;

        public WishlistService(ApplicationDbContext context, RequestContext requestContext)
        {
            _context = context;
            _requestContext = requestContext;
        }

        public async Task<List<WishlistItemVM>> GetMineAsync()
        {
            var userId = _requestContext.GetUserId();

            return await _context.WishlistItems
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Created)
                .Select(w => new WishlistItemVM
                {
                    Id = w.Id,
                    ProductId = w.ProductId,
                    Name = w.Product!.Name,
                    Slug = w.Product.Slug,
                    ThumbnailUrl = w.Product.Images!
                        .OrderByDescending(i => i.IsPrimary)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault(),
                    MinPrice = w.Product.Variants!.Min(v => (decimal?)v.Price),
                    Created = w.Created
                })
                .ToListAsync();
        }

        public async Task<bool> ToggleAsync(Guid productId)
        {
            var userId = _requestContext.GetUserId();

            var existing = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (existing != null)
            {
                _context.WishlistItems.Remove(existing);
                await _context.SaveChangesAsync();
                return false;
            }

            if (!await _context.Products.AnyAsync(p => p.Id == productId))
                throw new NotFoundException("Không tìm thấy sản phẩm");

            _context.WishlistItems.Add(new WishlistItem { UserId = userId, ProductId = productId });
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
