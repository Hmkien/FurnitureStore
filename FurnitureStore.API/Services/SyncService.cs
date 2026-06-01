using FurnitureStore.API.Common.Exceptions;
using FurnitureStore.API.Data;
using FurnitureStore.API.Extensions;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Middlewares;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Enums;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.Options;
using FurnitureStore.API.Repositories;
using FurnitureStore.API.Services.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FurnitureStore.API.Services
{
    public class SyncService : ISyncService
    {
        private readonly IConfiguration _config;
        private readonly ICategoryService _categories;
        private readonly IProductService _products;
        private readonly IProductVariantService _variants;
        private readonly IPostService _posts;
        private readonly IBannerService _banners;
        private readonly ISlideService _slides;
        private readonly ICouponService _coupons;
        private readonly IMembershipTierService _tiers;
        private readonly IRepository<Category> _categoryRepo;
        private readonly ApplicationDbContext _context;
        private readonly RequestContext _requestContext;
        private readonly ShippingOptions _shipping;

        private static readonly Random Rng = Random.Shared;

        public SyncService(
            IConfiguration config,
            ICategoryService categories,
            IProductService products,
            IProductVariantService variants,
            IPostService posts,
            IBannerService banners,
            ISlideService slides,
            ICouponService coupons,
            IMembershipTierService tiers,
            IRepository<Category> categoryRepo,
            ApplicationDbContext context,
            RequestContext requestContext,
            IOptions<ShippingOptions> shippingOptions)
        {
            _config = config;
            _categories = categories;
            _products = products;
            _variants = variants;
            _posts = posts;
            _banners = banners;
            _slides = slides;
            _coupons = coupons;
            _tiers = tiers;
            _categoryRepo = categoryRepo;
            _context = context;
            _requestContext = requestContext;
            _shipping = shippingOptions.Value;
        }

        public bool Enabled => _config.GetValue<bool>("EnableSync");

        public async Task<int> GenerateAsync(string resource, int count)
        {
            if (!Enabled)
                throw new BadRequestException("Tính năng giả lập dữ liệu đang tắt (EnableSync = false)");

            count = Math.Clamp(count, 1, 50);

            return resource.ToLowerInvariant() switch
            {
                "category" => await GenCategories(count),
                "product" => await GenProducts(count),
                "post" => await GenPosts(count),
                "banner" => await GenBanners(count),
                "slide" => await GenSlides(count),
                "coupon" => await GenCoupons(count),
                "membershiptier" => await GenTiers(count),
                _ => throw new BadRequestException($"Resource '{resource}' không hỗ trợ giả lập"),
            };
        }

        // ===== Seed sản phẩm thật từ moho.com.vn =====
        /// <summary>
        /// Mỗi lần gọi tạo ra <paramref name="count"/> sản phẩm Moho KHÁC NHAU (chưa từng tạo),
        /// kèm danh mục (phòng → loại), 1 biến thể và ảnh. Mặc định 5 sản phẩm/lần.
        /// </summary>
        public async Task<int> SeedMohoProductsAsync(int count)
        {
            if (!Enabled)
                throw new BadRequestException("Tính năng giả lập dữ liệu đang tắt (EnableSync = false)");

            count = Math.Clamp(count, 1, 20);

            var existingSlugs = (await _context.Products.Select(p => p.Slug).ToListAsync())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Chọn ngẫu nhiên các sản phẩm Moho CHƯA tạo → mỗi lần nhấn ra sản phẩm khác nhau.
            var pool = MohoSeedData.Products
                .Where(p => !existingSlugs.Contains(p.Name.ToSlug()))
                .OrderBy(_ => Rng.Next())
                .Take(count)
                .ToList();

            if (pool.Count == 0)
                return 0;

            var categoryMap = await EnsureMohoCategoriesAsync(pool.Select(p => p.Category));

            var created = 0;
            foreach (var seed in pool)
            {
                if (!categoryMap.TryGetValue(seed.Category, out var categoryId))
                    continue;

                _context.Products.Add(new Product
                {
                    CategoryId = categoryId,
                    Name = seed.Name,
                    Slug = seed.Name.ToSlug(),
                    Sku = $"MOHO-{Code(8)}",
                    ShortDescription = "Sản phẩm nội thất chính hãng MOHO.",
                    LongDescription = "Dữ liệu mẫu lấy từ moho.com.vn phục vụ demo cửa hàng nội thất.",
                    Style = "Hiện đại",
                    Status = StatusEntity.Approved,
                    Variants = new List<ProductVariant>
                    {
                        new()
                        {
                            Price = seed.Price,
                            StockQuantity = Rng.Next(8, 40),
                            Condition = ProductCondition.New,
                            Material = "Gỗ tự nhiên",
                            Color = "Tự nhiên",
                            SkuVariant = $"MOHO-V-{Code(8)}",
                            Status = StatusEntity.Approved,
                        }
                    },
                    Images = new List<ProductImage>
                    {
                        new() { ImageUrl = seed.ImageUrl, IsPrimary = true, Status = StatusEntity.Approved }
                    }
                });
                created++;
            }

            await _context.SaveChangesAsync();
            return created;
        }

        /// <summary>Đảm bảo danh mục cha (phòng) và con (loại sản phẩm) tồn tại; trả map: tên loại → CategoryId.</summary>
        private async Task<Dictionary<string, Guid>> EnsureMohoCategoriesAsync(IEnumerable<string> subNames)
        {
            var subs = subNames.Distinct().ToList();
            var byName = (await _context.Categories.ToListAsync())
                .GroupBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            // 1) Danh mục cha (phòng)
            foreach (var room in subs.Select(RoomOf).Distinct())
            {
                if (!byName.ContainsKey(room))
                {
                    var c = new Category { Name = room, Slug = room.ToSlug(), Status = StatusEntity.Approved };
                    _context.Categories.Add(c);
                    byName[room] = c;
                }
            }
            await _context.SaveChangesAsync();

            // 2) Danh mục con (loại sản phẩm) gắn vào phòng tương ứng
            foreach (var sub in subs)
            {
                if (!byName.ContainsKey(sub))
                {
                    var parent = byName[RoomOf(sub)];
                    var c = new Category { Name = sub, Slug = sub.ToSlug(), ParentId = parent.Id, Status = StatusEntity.Approved };
                    _context.Categories.Add(c);
                    byName[sub] = c;
                }
            }
            await _context.SaveChangesAsync();

            return subs.ToDictionary(s => s, s => byName[s].Id);
        }

        private static string RoomOf(string sub)
            => MohoSeedData.SubToRoom.TryGetValue(sub, out var room) ? room : "Khác";

        // ===== Giả lập đơn hàng với nhiều trạng thái khác nhau =====
        /// <summary>
        /// Tạo <paramref name="count"/> đơn hàng giả lập, trải đều qua tất cả trạng thái đơn,
        /// ngày tạo rải trong 30 ngày gần nhất. Không trừ tồn kho (chỉ là dữ liệu demo).
        /// </summary>
        public async Task<int> GenerateOrdersAsync(int count)
        {
            if (!Enabled)
                throw new BadRequestException("Tính năng giả lập dữ liệu đang tắt (EnableSync = false)");

            count = Math.Clamp(count, 1, 50);

            var variants = await _context.ProductVariants
                .Include(v => v.Product)
                .Where(v => v.Product != null)
                .Take(200)
                .ToListAsync();

            if (variants.Count == 0)
                throw new BadRequestException("Chưa có sản phẩm — hãy seed sản phẩm trước khi tạo đơn giả lập");

            var userIds = await _context.Users.Select(u => u.Id).Take(50).ToListAsync();
            if (userIds.Count == 0)
                userIds = new List<Guid> { _requestContext.GetUserId() };

            var statuses = Enum.GetValues<OrderStatus>();
            var orders = new List<Order>();

            for (var i = 0; i < count; i++)
            {
                var status = statuses[i % statuses.Length];
                var method = (PaymentMethod)Rng.Next(1, 5);

                var items = variants
                    .OrderBy(_ => Rng.Next())
                    .Take(Rng.Next(1, 4))
                    .Select(v =>
                    {
                        var qty = Rng.Next(1, 4);
                        return new OrderItem
                        {
                            VariantId = v.Id,
                            ProductName = v.Product!.Name,
                            VariantInfo = BuildVariantInfo(v),
                            Price = v.Price,
                            Quantity = qty,
                            LineTotal = v.Price * qty,
                            Status = StatusEntity.Approved,
                        };
                    })
                    .ToList();

                var subTotal = items.Sum(x => x.LineTotal);
                var shippingFee = ComputeShippingFee(subTotal);

                orders.Add(new Order
                {
                    OrderCode = $"ORD{DateTime.Now:yyMMddHHmmss}{Code(6)}",
                    UserId = userIds[Rng.Next(userIds.Count)],
                    SubTotal = subTotal,
                    ShippingFee = shippingFee,
                    DiscountAmount = 0,
                    TotalAmount = subTotal + shippingFee,
                    ReceiverName = Pick(CustomerNames),
                    ReceiverPhone = $"09{Rng.Next(10_000_000, 99_999_999)}",
                    ShippingAddress = Pick(Addresses),
                    Note = "Đơn hàng giả lập",
                    PaymentMethod = method,
                    PaymentStatus = MapPayment(status, method),
                    OrderStatus = status,
                    Status = StatusEntity.Approved,
                    Items = items,
                });
            }

            _context.Orders.AddRange(orders);
            await _context.SaveChangesAsync();

            // Rải ngày tạo trong 30 ngày gần nhất (ExecuteUpdate bỏ qua audit để ghi được Created).
            var now = DateTime.Now;
            foreach (var o in orders)
            {
                var createdAt = now.AddDays(-Rng.Next(0, 30)).AddHours(-Rng.Next(0, 24)).AddMinutes(-Rng.Next(0, 60));
                await _context.Orders.Where(x => x.Id == o.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.Created, createdAt));
            }

            return orders.Count;
        }

        private decimal ComputeShippingFee(decimal subTotal)
        {
            if (subTotal <= 0) return 0m;
            if (_shipping.FreeShippingThreshold > 0 && subTotal >= _shipping.FreeShippingThreshold) return 0m;
            return _shipping.FlatFee;
        }

        private static PaymentStatus MapPayment(OrderStatus status, PaymentMethod method) => status switch
        {
            OrderStatus.Completed => PaymentStatus.Paid,
            OrderStatus.Cancelled => PaymentStatus.Unpaid,
            OrderStatus.Shipping or OrderStatus.Preparing =>
                method == PaymentMethod.COD ? PaymentStatus.Unpaid : PaymentStatus.Paid,
            _ => PaymentStatus.Unpaid,
        };

        private static string BuildVariantInfo(ProductVariant v)
        {
            var parts = new[] { v.Size, v.Material, v.Color }.Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(" / ", parts);
        }

        private static readonly string[] CustomerNames =
            { "Nguyễn Văn An", "Trần Thị Bình", "Lê Hoàng Cường", "Phạm Thu Dung", "Vũ Minh Đức",
              "Hoàng Thị Hoa", "Đặng Quốc Khánh", "Bùi Thị Lan", "Đỗ Văn Mạnh", "Ngô Thị Ngọc" };
        private static readonly string[] Addresses =
            { "12 Lê Lợi, Q.1, TP.HCM", "88 Cầu Giấy, Hà Nội", "45 Nguyễn Văn Linh, Đà Nẵng",
              "230 Trần Hưng Đạo, Q.5, TP.HCM", "17 Bà Triệu, Hoàn Kiếm, Hà Nội", "9 Lê Duẩn, Hải Châu, Đà Nẵng" };

        // ===== Generators =====
        private async Task<int> GenCategories(int count)
        {
            var n = 0;
            for (var i = 0; i < count; i++)
            {
                await TryCreate(() => _categories.Create(new CategoryForm
                {
                    Name = $"{Pick(CategoryNames)} {Suffix()}",
                    Description = "Danh mục giả lập",
                    ImageUrl = Img(600, 400),
                }), () => n++)
                ;
            }
            return n;
        }

        private async Task<int> GenProducts(int count)
        {
            var categoryIds = await _categoryRepo.Query().Select(c => c.Id).ToListAsync();
            if (categoryIds.Count == 0)
            {
                await GenCategories(3);
                categoryIds = await _categoryRepo.Query().Select(c => c.Id).ToListAsync();
            }
            if (categoryIds.Count == 0) return 0;

            var n = 0;
            for (var i = 0; i < count; i++)
            {
                var name = $"{Pick(ProductNames)} {Pick(Styles)} {Suffix()}";
                try
                {
                    var product = await _products.Create(new ProductForm
                    {
                        CategoryId = categoryIds[Rng.Next(categoryIds.Count)],
                        Name = name,
                        Sku = $"SP{Code(6)}",
                        ShortDescription = "Sản phẩm nội thất giả lập",
                        LongDescription = "Mô tả chi tiết sản phẩm giả lập phục vụ demo.",
                        Style = Pick(Styles),
                    });
                    n++;

                    var variantCount = Rng.Next(1, 4);
                    for (var v = 0; v < variantCount; v++)
                    {
                        await TryCreate(() => _variants.Create(new ProductVariantForm
                        {
                            ProductId = product.Id,
                            Size = Pick(Sizes),
                            Material = Pick(Materials),
                            Color = Pick(Colors),
                            Condition = (ProductCondition)Rng.Next(1, 6),
                            Price = Rng.Next(15, 300) * 100_000,
                            StockQuantity = Rng.Next(0, 50),
                            SkuVariant = $"BT{Code(7)}",
                        }), () => { });
                    }
                }
                catch (Exception ex) when (ex is AppException) { /* bỏ qua bản ghi lỗi */ }
            }
            return n;
        }

        private async Task<int> GenPosts(int count)
        {
            var n = 0;
            for (var i = 0; i < count; i++)
            {
                await TryCreate(() => _posts.Create(new PostForm
                {
                    Title = $"{Pick(PostTitles)} {Suffix()}",
                    Summary = "Tóm tắt bài viết giả lập.",
                    Content = "Nội dung bài viết giả lập phục vụ demo trang tin tức.",
                    ThumbnailUrl = Img(800, 500),
                    Category = Pick(PostCategories),
                    Author = "Admin",
                    IsPublished = Rng.Next(2) == 1,
                }), () => n++);
            }
            return n;
        }

        private async Task<int> GenBanners(int count)
        {
            var n = 0;
            for (var i = 0; i < count; i++)
            {
                await TryCreate(() => _banners.Create(new BannerForm
                {
                    Title = $"Banner {Pick(PostTitles)} {Suffix()}",
                    ImageUrl = Img(1200, 400),
                    LinkUrl = "/khuyen-mai",
                    Position = Pick(new[] { "home_top", "sidebar", "category" }),
                    SortOrder = Rng.Next(0, 10),
                }), () => n++);
            }
            return n;
        }

        private async Task<int> GenSlides(int count)
        {
            var n = 0;
            for (var i = 0; i < count; i++)
            {
                await TryCreate(() => _slides.Create(new SlideForm
                {
                    Title = $"Slide {Pick(PostTitles)} {Suffix()}",
                    ImageUrl = Img(1600, 600),
                    Caption = "Ưu đãi nội thất giả lập",
                    LinkUrl = "/bo-suu-tap",
                    SortOrder = Rng.Next(0, 10),
                }), () => n++);
            }
            return n;
        }

        private async Task<int> GenCoupons(int count)
        {
            var n = 0;
            for (var i = 0; i < count; i++)
            {
                var percentage = Rng.Next(2) == 1;
                await TryCreate(() => _coupons.Create(new CouponForm
                {
                    Code = $"SALE{Code(5)}",
                    Description = "Mã giảm giá giả lập",
                    DiscountType = percentage ? DiscountType.Percentage : DiscountType.Fixed,
                    DiscountValue = percentage ? Rng.Next(5, 30) : Rng.Next(50, 300) * 1000,
                    MinOrderAmount = Rng.Next(0, 10) * 100_000,
                    MaxDiscount = percentage ? Rng.Next(1, 5) * 100_000 : 0,
                    UsageLimit = Rng.Next(0, 100),
                }), () => n++);
            }
            return n;
        }

        private async Task<int> GenTiers(int count)
        {
            var n = 0;
            for (var i = 0; i < count; i++)
            {
                await TryCreate(() => _tiers.Create(new MembershipTierForm
                {
                    Name = $"{Pick(TierNames)} {Suffix()}",
                    MinSpending = Rng.Next(0, 50) * 1_000_000,
                    DiscountPercent = Rng.Next(0, 15),
                    Description = "Hạng thành viên giả lập",
                }), () => n++);
            }
            return n;
        }

        // ===== Helpers =====
        private static async Task TryCreate(Func<Task> create, Action onOk)
        {
            try
            {
                await create();
                onOk();
            }
            catch (AppException)
            {
                // trùng mã/slug ngẫu nhiên -> bỏ qua
            }
        }

        private static string Pick(string[] arr) => arr[Rng.Next(arr.Length)];
        private static string Suffix() => Rng.Next(1, 9999).ToString();
        private static string Code(int len)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            return new string(Enumerable.Range(0, len).Select(_ => chars[Rng.Next(chars.Length)]).ToArray());
        }
        private static string Img(int w, int h) => $"https://picsum.photos/seed/{Code(6)}/{w}/{h}";

        private static readonly string[] CategoryNames =
            { "Sofa", "Bàn ăn", "Giường ngủ", "Tủ quần áo", "Kệ tivi", "Bàn làm việc", "Ghế thư giãn", "Tủ bếp", "Kệ sách", "Bàn trà" };
        private static readonly string[] ProductNames =
            { "Sofa góc", "Bàn ăn mặt đá", "Giường bọc nệm", "Tủ áo cánh lùa", "Kệ tivi gỗ", "Ghế sofa đơn", "Bàn trà tròn", "Tủ trang trí" };
        private static readonly string[] Styles = { "Hiện đại", "Cổ điển", "Tối giản", "Bắc Âu", "Tân cổ điển" };
        private static readonly string[] Materials = { "Gỗ sồi", "Gỗ óc chó", "MDF phủ Melamine", "Da công nghiệp", "Vải nỉ", "Kim loại sơn tĩnh điện" };
        private static readonly string[] Colors = { "Nâu gỗ", "Trắng", "Đen", "Xám", "Be", "Xanh rêu" };
        private static readonly string[] Sizes = { "1m6", "1m8", "2m", "Nhỏ", "Vừa", "Lớn" };
        private static readonly string[] PostTitles =
            { "Xu hướng nội thất", "Mẹo phối màu phòng khách", "Chọn sofa cho căn hộ nhỏ", "Bảo quản đồ gỗ", "Phong cách Bắc Âu" };
        private static readonly string[] PostCategories = { "Tin tức", "Xu hướng", "Mẹo hay", "Khuyến mãi" };
        private static readonly string[] TierNames = { "Đồng", "Bạc", "Vàng", "Kim cương" };
    }
}
