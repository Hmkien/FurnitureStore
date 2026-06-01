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
    public class CartService : ICartService
    {
        private const string CartTokenHeader = "X-Cart-Token";

        private readonly ApplicationDbContext _context;
        private readonly RequestContext _requestContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(
            ApplicationDbContext context,
            RequestContext requestContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _requestContext = requestContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CartVM> GetMyCartAsync()
        {
            var cart = await GetOrCreateCartAsync();
            return await BuildCartVmAsync(cart.Id);
        }

        public async Task<CartVM> AddItemAsync(AddToCartForm form)
        {
            var variant = await _context.ProductVariants.FindAsync(form.VariantId)
                ?? throw new NotFoundException("Không tìm thấy biến thể sản phẩm");

            var cart = await GetOrCreateCartAsync();

            var item = await _context.CartItems
                .FirstOrDefaultAsync(i => i.CartId == cart.Id && i.VariantId == form.VariantId);

            var newQuantity = (item?.Quantity ?? 0) + form.Quantity;
            EnsureInStock(variant, newQuantity);

            if (item == null)
            {
                _context.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
                    VariantId = form.VariantId,
                    Quantity = form.Quantity
                });
            }
            else
            {
                item.Quantity = newQuantity;
            }

            await _context.SaveChangesAsync();
            return await BuildCartVmAsync(cart.Id);
        }

        public async Task<CartVM> UpdateItemAsync(Guid cartItemId, int quantity)
        {
            if (quantity < 1)
                throw new BadRequestException("Số lượng phải >= 1");

            var item = await GetOwnedItemAsync(cartItemId);

            var variant = await _context.ProductVariants.FindAsync(item.VariantId)
                ?? throw new NotFoundException("Không tìm thấy biến thể sản phẩm");
            EnsureInStock(variant, quantity);

            item.Quantity = quantity;
            await _context.SaveChangesAsync();
            return await BuildCartVmAsync(item.CartId);
        }

        public async Task<CartVM> RemoveItemAsync(Guid cartItemId)
        {
            var item = await GetOwnedItemAsync(cartItemId);
            var cartId = item.CartId;

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return await BuildCartVmAsync(cartId);
        }

        public async Task ClearAsync()
        {
            var (userId, token) = ResolveOwner();

            var query = _context.CartItems.AsQueryable();
            query = userId != null
                ? query.Where(i => i.Cart!.UserId == userId)
                : query.Where(i => i.Cart!.GuestToken == token);

            var items = await query.ToListAsync();
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task<CartVM> MergeAsync(string guestToken)
        {
            if (!_requestContext.IsAuthenticated)
                throw new UnauthorizedAccessException("Cần đăng nhập để gộp giỏ hàng");

            var userId = _requestContext.GetUserId();

            var userCart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (userCart == null)
            {
                userCart = new Cart { UserId = userId };
                _context.Carts.Add(userCart);
                await _context.SaveChangesAsync();
            }

            if (!string.IsNullOrWhiteSpace(guestToken))
            {
                var guestCart = await _context.Carts.FirstOrDefaultAsync(c => c.GuestToken == guestToken);

                if (guestCart != null && guestCart.Id != userCart.Id)
                {
                    var guestItems = await _context.CartItems
                        .Where(i => i.CartId == guestCart.Id).ToListAsync();
                    var userItems = await _context.CartItems
                        .Where(i => i.CartId == userCart.Id).ToListAsync();

                    foreach (var gItem in guestItems)
                    {
                        var variant = await _context.ProductVariants.FindAsync(gItem.VariantId);
                        var existing = userItems.FirstOrDefault(u => u.VariantId == gItem.VariantId);

                        // Variant không còn hoặc hết hàng → bỏ qua dòng guest.
                        if (variant == null || variant.StockQuantity <= 0)
                        {
                            _context.CartItems.Remove(gItem);
                            continue;
                        }

                        if (existing != null)
                        {
                            // Gộp số lượng, clamp theo tồn kho (tránh vi phạm unique (CartId, VariantId)).
                            existing.Quantity = Math.Min(existing.Quantity + gItem.Quantity, variant.StockQuantity);
                            _context.CartItems.Remove(gItem);
                        }
                        else
                        {
                            // Chuyển dòng guest sang giỏ user.
                            gItem.CartId = userCart.Id;
                            gItem.Quantity = Math.Min(gItem.Quantity, variant.StockQuantity);
                        }
                    }

                    _context.Carts.Remove(guestCart);
                    await _context.SaveChangesAsync();
                }
            }

            return await BuildCartVmAsync(userCart.Id);
        }

        /// <summary>
        /// Xác định chủ giỏ: user đã đăng nhập, hoặc khách ẩn danh qua header X-Cart-Token.
        /// </summary>
        private (Guid? userId, string? token) ResolveOwner()
        {
            if (_requestContext.IsAuthenticated)
                return (_requestContext.GetUserId(), null);

            var token = _httpContextAccessor.HttpContext?.Request.Headers[CartTokenHeader].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(token))
                throw new BadRequestException("Thiếu X-Cart-Token cho giỏ hàng khách");

            return (null, token);
        }

        private async Task<Cart> GetOrCreateCartAsync()
        {
            var (userId, token) = ResolveOwner();

            var cart = userId != null
                ? await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId)
                : await _context.Carts.FirstOrDefaultAsync(c => c.GuestToken == token);

            if (cart != null)
                return cart;

            cart = new Cart { UserId = userId, GuestToken = token };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        private async Task<CartItem> GetOwnedItemAsync(Guid cartItemId)
        {
            var (userId, token) = ResolveOwner();

            var query = _context.CartItems.Where(i => i.Id == cartItemId);
            query = userId != null
                ? query.Where(i => i.Cart!.UserId == userId)
                : query.Where(i => i.Cart!.GuestToken == token);

            return await query.FirstOrDefaultAsync()
                ?? throw new NotFoundException("Không tìm thấy sản phẩm trong giỏ");
        }

        private static void EnsureInStock(ProductVariant variant, int requestedQuantity)
        {
            if (requestedQuantity > variant.StockQuantity)
                throw new BadRequestException($"Chỉ còn {variant.StockQuantity} sản phẩm trong kho");
        }

        private async Task<CartVM> BuildCartVmAsync(Guid cartId)
        {
            var items = await _context.CartItems
                .Where(i => i.CartId == cartId)
                .Select(i => new CartItemVM
                {
                    Id = i.Id,
                    VariantId = i.VariantId,
                    ProductId = i.Variant!.ProductId,
                    ProductName = i.Variant.Product!.Name,
                    Size = i.Variant.Size,
                    Material = i.Variant.Material,
                    Color = i.Variant.Color,
                    ImageUrl = i.Variant.Product.Images!
                        .OrderByDescending(img => img.IsPrimary)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault(),
                    Price = i.Variant.Price,
                    Quantity = i.Quantity,
                    LineTotal = i.Variant.Price * i.Quantity,
                    StockQuantity = i.Variant.StockQuantity
                })
                .ToListAsync();

            return new CartVM
            {
                Id = cartId,
                Items = items,
                TotalItems = items.Sum(i => i.Quantity),
                SubTotal = items.Sum(i => i.LineTotal)
            };
        }
    }
}
