using FurnitureStore.API.Common.Exceptions;
using FurnitureStore.API.Extensions;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Enums;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.Query;
using FurnitureStore.API.Models.ViewModels;
using FurnitureStore.API.Repositories;
using FurnitureStore.API.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Services
{
    public class ProductService : CrudService<Product, ProductForm>, IProductService
    {
        public ProductService(IRepository<Product> repository) : base(repository)
        {
        }

        protected override async Task ValidateAsync(ProductForm form, Guid? existingId = null)
        {
            if (await Repository.AnyAsync(p => p.Sku == form.Sku && p.Id != existingId))
                throw new ConflictException($"SKU '{form.Sku}' đã tồn tại");

            var slug = BuildSlug(form);
            if (await Repository.AnyAsync(p => p.Slug == slug && p.Id != existingId))
                throw new ConflictException($"Slug '{slug}' đã tồn tại");
        }

        public override async Task<DataTableJson> GetPaged(BaseQuery query)
        {
            var builder = ApplyFilters(
                Repository.Query().ApplyQuery(query).WithDynamicSearch().WithDateFilter(x => x.Created),
                query);

            // Sắp xếp theo giá phải xử lý riêng: giá nằm ở ProductVariant, WithSort chỉ reflect trên Product.
            var sortBy = query.SortBy ?? "Created";
            IQueryable<Product> filtered;
            if (sortBy.Equals("price", StringComparison.OrdinalIgnoreCase))
            {
                var q = builder.GetQuery();
                filtered = query.SortDesc
                    ? q.OrderByDescending(p => p.Variants!.Min(v => (decimal?)v.Price))
                    : q.OrderBy(p => p.Variants!.Min(v => (decimal?)v.Price));
            }
            else
            {
                filtered = builder.WithSort("Created").GetQuery();
            }

            var recordsTotal = await Repository.Query().CountAsync();
            var recordsFiltered = await filtered.CountAsync();
            var data = await filtered
                .Paginate(query)
                .Select(p => new ProductListVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    Sku = p.Sku,
                    Style = p.Style,
                    CategoryId = p.CategoryId,
                    Status = p.Status,
                    Created = p.Created,
                    MinPrice = p.Variants!.Min(v => (decimal?)v.Price),
                    ThumbnailUrl = p.Images!.OrderByDescending(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault(),
                })
                .ToListAsync();

            return new DataTableJson
            {
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        protected override QueryBuilder<Product> ApplyFilters(QueryBuilder<Product> builder, BaseQuery query)
        {
            if (query is not ProductQuery q)
                return builder;

            return builder
                .WithFilterIf(q.CategoryId.HasValue, p => p.CategoryId == q.CategoryId!.Value)
                .WithFilterIf(!string.IsNullOrWhiteSpace(q.Style), p => p.Style == q.Style)
                .WithFilterIf(q.MinPrice.HasValue,
                    p => p.Variants!.Any(v => v.Price >= q.MinPrice!.Value))
                .WithFilterIf(q.MaxPrice.HasValue,
                    p => p.Variants!.Any(v => v.Price <= q.MaxPrice!.Value))
                .WithFilterIf(!string.IsNullOrWhiteSpace(q.Material),
                    p => p.Variants!.Any(v => v.Material == q.Material))
                .WithFilterIf(q.Condition.HasValue,
                    p => p.Variants!.Any(v => v.Condition == q.Condition!.Value));
        }

        public async Task<ProductDetailVM> GetDetailAsync(Guid id)
        {
            var detail = await Repository.Query()
                .Where(p => p.Id == id)
                .Select(p => new ProductDetailVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    Sku = p.Sku,
                    ShortDescription = p.ShortDescription,
                    LongDescription = p.LongDescription,
                    Style = p.Style,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category!.Name,
                    MinPrice = p.Variants!.Min(v => (decimal?)v.Price),
                    MaxPrice = p.Variants!.Max(v => (decimal?)v.Price),
                    Variants = p.Variants!.Select(v => new ProductVariantVM
                    {
                        Id = v.Id,
                        Size = v.Size,
                        Material = v.Material,
                        Color = v.Color,
                        Condition = v.Condition,
                        Price = v.Price,
                        StockQuantity = v.StockQuantity,
                        SkuVariant = v.SkuVariant
                    }).ToList(),
                    Images = p.Images!.Select(i => new ProductImageVM
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        IsPrimary = i.IsPrimary
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return detail ?? throw new NotFoundException("Không tìm thấy sản phẩm");
        }

        public async Task<ProductFilterOptionsVM> GetFilterOptionsAsync()
        {
            var approved = Repository.Query().Where(p => p.Status == StatusEntity.Approved);

            var styles = await approved
                .Where(p => p.Style != null && p.Style != "")
                .Select(p => p.Style!)
                .Distinct().OrderBy(s => s).ToListAsync();

            var variants = approved.SelectMany(p => p.Variants!);

            var materials = await variants
                .Where(v => v.Material != null && v.Material != "")
                .Select(v => v.Material!)
                .Distinct().OrderBy(m => m).ToListAsync();

            var colors = await variants
                .Where(v => v.Color != null && v.Color != "")
                .Select(v => v.Color!)
                .Distinct().OrderBy(c => c).ToListAsync();

            var prices = variants.Select(v => (decimal?)v.Price);

            return new ProductFilterOptionsVM
            {
                Materials = materials,
                Colors = colors,
                Styles = styles,
                MinPrice = await prices.MinAsync(),
                MaxPrice = await prices.MaxAsync()
            };
        }

        protected override Product MapToNew(ProductForm form) => new()
        {
            CategoryId = form.CategoryId,
            Name = form.Name,
            Slug = BuildSlug(form),
            Sku = form.Sku,
            ShortDescription = form.ShortDescription,
            LongDescription = form.LongDescription,
            Style = form.Style
        };

        protected override void ApplyUpdate(Product entity, ProductForm form)
        {
            entity.CategoryId = form.CategoryId;
            entity.Name = form.Name;
            entity.Slug = BuildSlug(form);
            entity.Sku = form.Sku;
            entity.ShortDescription = form.ShortDescription;
            entity.LongDescription = form.LongDescription;
            entity.Style = form.Style;
        }

        private static string BuildSlug(ProductForm form)
            => string.IsNullOrWhiteSpace(form.Slug) ? form.Name.ToSlug() : form.Slug.ToSlug();
    }
}
