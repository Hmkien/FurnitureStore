using FurnitureStore.API.Common.Exceptions;
using FurnitureStore.API.Data;
using FurnitureStore.API.Extensions;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Middlewares;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Enums;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Models.Query;
using FurnitureStore.API.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly RequestContext _requestContext;

        public InventoryService(ApplicationDbContext context, RequestContext requestContext)
        {
            _context = context;
            _requestContext = requestContext;
        }

        public async Task<ProductVariant> AdjustAsync(StockAdjustForm form)
        {
            EnsureManager();

            var variant = await _context.ProductVariants.FindAsync(form.VariantId)
                ?? throw new NotFoundException("Không tìm thấy biến thể sản phẩm");

            var delta = form.Type == StockMovementType.Export ? -form.Quantity : form.Quantity;
            var newStock = variant.StockQuantity + delta;
            if (newStock < 0)
                throw new BadRequestException($"Tồn kho không đủ (hiện còn {variant.StockQuantity})");

            variant.StockQuantity = newStock;

            _context.StockMovements.Add(new StockMovement
            {
                VariantId = variant.Id,
                Type = form.Type,
                Quantity = form.Quantity,
                QuantityAfter = newStock,
                Note = form.Note
            });

            await _context.SaveChangesAsync();
            return variant;
        }

        public async Task<DataTableJson> GetHistoryAsync(Guid variantId, BaseQuery query)
        {
            EnsureManager();

            var source = _context.StockMovements.Where(m => m.VariantId == variantId);

            var filtered = source
                .ApplyQuery(query)
                .WithDateFilter(x => x.Created)
                .WithSort("Created")
                .GetQuery();

            var recordsTotal = await source.CountAsync();
            var recordsFiltered = await filtered.CountAsync();
            var data = await filtered
                .Paginate(query)
                .Select(m => new StockMovementVM
                {
                    Id = m.Id,
                    VariantId = m.VariantId,
                    SkuVariant = m.Variant!.SkuVariant,
                    ProductName = m.Variant.Product!.Name,
                    Type = m.Type,
                    Quantity = m.Quantity,
                    QuantityAfter = m.QuantityAfter,
                    Note = m.Note,
                    Created = m.Created
                })
                .ToListAsync();

            return new DataTableJson
            {
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        public async Task<List<LowStockVariantVM>> GetLowStockAsync(int threshold)
        {
            EnsureManager();
            if (threshold < 0) threshold = 0;

            return await _context.ProductVariants
                .Where(v => v.StockQuantity <= threshold)
                .OrderBy(v => v.StockQuantity)
                .Select(v => new LowStockVariantVM
                {
                    VariantId = v.Id,
                    ProductId = v.ProductId,
                    ProductName = v.Product!.Name,
                    SkuVariant = v.SkuVariant,
                    StockQuantity = v.StockQuantity
                })
                .ToListAsync();
        }

        private void EnsureManager()
        {
            if (!_requestContext.IsSuperUser() && !_requestContext.HasPermission("INVENTORY_MANAGE"))
                throw new UnauthorizedAccessException("Bạn không có quyền quản lý kho");
        }
    }
}
