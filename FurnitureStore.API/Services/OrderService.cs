using FurnitureStore.API.Common.Exceptions;
using FurnitureStore.API.Data;
using FurnitureStore.API.Extensions;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Middlewares;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Enums;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.Options;
using FurnitureStore.API.Models.Query;
using FurnitureStore.API.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FurnitureStore.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly RequestContext _requestContext;
        private readonly ICouponService _couponService;
        private readonly ShippingOptions _shipping;

        public OrderService(
            ApplicationDbContext context,
            RequestContext requestContext,
            ICouponService couponService,
            IOptions<ShippingOptions> shippingOptions)
        {
            _context = context;
            _requestContext = requestContext;
            _couponService = couponService;
            _shipping = shippingOptions.Value;
        }

        public async Task<OrderDetailVM> CheckoutAsync(CheckoutForm form)
        {
            var userId = _requestContext.GetUserId();

            var cartItems = await _context.CartItems
                .Include(i => i.Variant!).ThenInclude(v => v.Product)
                .Where(i => i.Cart!.UserId == userId)
                .ToListAsync();

            if (cartItems.Count == 0)
                throw new BadRequestException("Giỏ hàng đang trống");

            var order = new Order
            {
                OrderCode = GenerateOrderCode(),
                UserId = userId,
                ReceiverName = form.ReceiverName,
                ReceiverPhone = form.ReceiverPhone,
                ShippingAddress = form.ShippingAddress,
                Note = form.Note,
                PaymentMethod = form.PaymentMethod,
                PaymentStatus = PaymentStatus.Unpaid,
                OrderStatus = OrderStatus.Pending,
                Items = new List<OrderItem>()
            };

            foreach (var item in cartItems)
            {
                var variant = item.Variant!;
                if (item.Quantity > variant.StockQuantity)
                    throw new BadRequestException(
                        $"Sản phẩm '{variant.Product!.Name}' chỉ còn {variant.StockQuantity} trong kho");

                order.Items.Add(new OrderItem
                {
                    VariantId = variant.Id,
                    ProductName = variant.Product!.Name,
                    VariantInfo = BuildVariantInfo(variant),
                    Price = variant.Price,
                    Quantity = item.Quantity,
                    LineTotal = variant.Price * item.Quantity
                });

                variant.StockQuantity -= item.Quantity;
            }

            order.SubTotal = order.Items.Sum(i => i.LineTotal);

            if (!string.IsNullOrWhiteSpace(form.CouponCode))
            {
                var (coupon, discount) = await _couponService.ValidateAndComputeAsync(form.CouponCode, order.SubTotal);
                order.CouponCode = coupon.Code;
                order.DiscountAmount = discount;
                coupon.UsedCount += 1; // cùng DbContext scope → lưu chung khi SaveChanges
            }

            // Phí ship tính server-side từ tạm tính thực tế (không tin client).
            order.ShippingFee = ComputeShippingFee(order.SubTotal);
            order.TotalAmount = order.SubTotal - order.DiscountAmount + order.ShippingFee;

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);

            // Một SaveChanges = một transaction: tạo đơn, trừ kho, xóa giỏ là nguyên tử.
            await _context.SaveChangesAsync();

            return await GetDetailAsync(order.Id);
        }

        public ShippingQuoteVM QuoteShipping(decimal subTotal)
        {
            if (subTotal < 0) subTotal = 0;
            var fee = ComputeShippingFee(subTotal);
            return new ShippingQuoteVM
            {
                SubTotal = subTotal,
                ShippingFee = fee,
                FreeShippingThreshold = _shipping.FreeShippingThreshold,
                IsFreeShipping = fee == 0m && subTotal > 0
            };
        }

        /// <summary>Phí ship: miễn phí nếu đạt ngưỡng (>0), ngược lại phí cố định. Giỏ trống = 0.</summary>
        private decimal ComputeShippingFee(decimal subTotal)
        {
            if (subTotal <= 0) return 0m;
            if (_shipping.FreeShippingThreshold > 0 && subTotal >= _shipping.FreeShippingThreshold)
                return 0m;
            return _shipping.FlatFee;
        }

        public Task<DataTableJson> GetMyOrdersAsync(OrderQuery query)
        {
            var userId = _requestContext.GetUserId();
            return BuildPagedAsync(query, _context.Orders.Where(o => o.UserId == userId));
        }

        public Task<DataTableJson> GetAllAsync(BaseQuery query)
        {
            EnsureManager();
            return BuildPagedAsync(query, _context.Orders);
        }

        public async Task<OrderDetailVM> GetDetailAsync(Guid id)
        {
            var detail = await _context.Orders
                .Where(o => o.Id == id)
                .Select(o => new OrderDetailVM
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    SubTotal = o.SubTotal,
                    ShippingFee = o.ShippingFee,
                    DiscountAmount = o.DiscountAmount,
                    CouponCode = o.CouponCode,
                    TotalAmount = o.TotalAmount,
                    PaymentMethod = o.PaymentMethod,
                    PaymentStatus = o.PaymentStatus,
                    OrderStatus = o.OrderStatus,
                    Created = o.Created,
                    ItemCount = o.Items!.Count,
                    ReceiverName = o.ReceiverName,
                    ReceiverPhone = o.ReceiverPhone,
                    ShippingAddress = o.ShippingAddress,
                    Note = o.Note,
                    UserId = o.UserId,
                    Items = o.Items!.Select(i => new OrderItemVM
                    {
                        Id = i.Id,
                        VariantId = i.VariantId,
                        ProductName = i.ProductName,
                        VariantInfo = i.VariantInfo,
                        Price = i.Price,
                        Quantity = i.Quantity,
                        LineTotal = i.LineTotal
                    }).ToList()
                })
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Không tìm thấy đơn hàng");

            EnsureOwnerOrManager(detail.UserId);
            return detail;
        }

        public async Task UpdateStatusAsync(Guid id, OrderStatus status)
        {
            EnsureManager();
            var order = await FindOrderAsync(id);

            if (order.OrderStatus == OrderStatus.Cancelled)
                throw new BadRequestException("Đơn đã hủy, không thể đổi trạng thái");

            order.OrderStatus = status;
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePaymentStatusAsync(Guid id, PaymentStatus status)
        {
            EnsureManager();
            var order = await FindOrderAsync(id);
            order.PaymentStatus = status;
            await _context.SaveChangesAsync();
        }

        public async Task CancelAsync(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.Items!)
                .FirstOrDefaultAsync(o => o.Id == id)
                ?? throw new NotFoundException("Không tìm thấy đơn hàng");

            EnsureOwnerOrManager(order.UserId);

            if (order.OrderStatus is OrderStatus.Shipping or OrderStatus.Completed)
                throw new BadRequestException("Đơn đang giao hoặc đã hoàn thành, không thể hủy");
            if (order.OrderStatus == OrderStatus.Cancelled)
                throw new BadRequestException("Đơn đã được hủy trước đó");

            // Hoàn lại tồn kho
            var variantIds = order.Items!.Select(i => i.VariantId).ToList();
            var variants = await _context.ProductVariants
                .Where(v => variantIds.Contains(v.Id))
                .ToListAsync();

            foreach (var item in order.Items!)
            {
                var variant = variants.FirstOrDefault(v => v.Id == item.VariantId);
                if (variant != null)
                    variant.StockQuantity += item.Quantity;
            }

            order.OrderStatus = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();
        }

        private async Task<DataTableJson> BuildPagedAsync(BaseQuery query, IQueryable<Order> source)
        {
            var orderQuery = query as OrderQuery;
            var filtered = source
                .ApplyQuery(query)
                .WithDynamicSearch()
                .WithFilterIf(orderQuery?.OrderStatus != null, o => o.OrderStatus == orderQuery!.OrderStatus)
                .WithFilterIf(orderQuery?.PaymentStatus != null, o => o.PaymentStatus == orderQuery!.PaymentStatus)
                .WithDateFilter(x => x.Created)
                .WithSort("Created")
                .GetQuery();

            var recordsTotal = await source.CountAsync();
            var recordsFiltered = await filtered.CountAsync();
            var data = await filtered
                .Paginate(query)
                .Select(o => new OrderVM
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    SubTotal = o.SubTotal,
                    ShippingFee = o.ShippingFee,
                    DiscountAmount = o.DiscountAmount,
                    CouponCode = o.CouponCode,
                    TotalAmount = o.TotalAmount,
                    PaymentMethod = o.PaymentMethod,
                    PaymentStatus = o.PaymentStatus,
                    OrderStatus = o.OrderStatus,
                    ItemCount = o.Items!.Count,
                    Created = o.Created,
                    FirstProductName = o.Items!.Select(i => i.ProductName).FirstOrDefault(),
                    ThumbnailUrl = o.Items!
                        .Select(i => i.Variant!.Product!.Images!
                            .OrderByDescending(im => im.IsPrimary)
                            .Select(im => im.ImageUrl)
                            .FirstOrDefault())
                        .FirstOrDefault()
                })
                .ToListAsync();

            return new DataTableJson
            {
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        private async Task<Order> FindOrderAsync(Guid id)
            => await _context.Orders.FirstOrDefaultAsync(o => o.Id == id)
                ?? throw new NotFoundException("Không tìm thấy đơn hàng");

        private void EnsureManager()
        {
            if (!_requestContext.IsSuperUser() && !_requestContext.HasPermission("ORDER_MANAGE"))
                throw new UnauthorizedAccessException("Bạn không có quyền quản lý đơn hàng");
        }

        private void EnsureOwnerOrManager(Guid ownerId)
        {
            if (_requestContext.GetUserId() == ownerId)
                return;
            if (_requestContext.IsSuperUser() || _requestContext.HasPermission("ORDER_MANAGE"))
                return;

            throw new UnauthorizedAccessException("Bạn không có quyền xem đơn hàng này");
        }

        private static string BuildVariantInfo(ProductVariant variant)
        {
            var parts = new[] { variant.Size, variant.Material, variant.Color }
                .Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(" / ", parts);
        }

        private static string GenerateOrderCode()
            => $"ORD{DateTime.Now:yyMMddHHmmss}{Guid.NewGuid().ToString("N")[..4].ToUpper()}";
    }
}
