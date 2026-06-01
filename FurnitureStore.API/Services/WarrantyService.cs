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
    public class WarrantyService : IWarrantyService
    {
        private readonly ApplicationDbContext _context;
        private readonly RequestContext _requestContext;

        public WarrantyService(ApplicationDbContext context, RequestContext requestContext)
        {
            _context = context;
            _requestContext = requestContext;
        }

        public async Task<ReturnRequestResult> CreateAsync(CreateWarrantyForm form)
        {
            var userId = _requestContext.GetUserId();

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == form.OrderId)
                ?? throw new NotFoundException("Không tìm thấy đơn hàng");
            if (order.UserId != userId && !IsManager())
                throw new UnauthorizedAccessException("Không có quyền tạo phiếu cho đơn này");

            var variant = await _context.ProductVariants.Include(v => v.Product).FirstOrDefaultAsync(v => v.Id == form.VariantId)
                ?? throw new NotFoundException("Không tìm thấy sản phẩm");

            var entity = new WarrantyClaim
            {
                Code = $"WR{DateTime.Now:yyMMddHHmmss}",
                OrderId = order.Id,
                UserId = userId,
                VariantId = variant.Id,
                ProductName = variant.Product?.Name ?? "Sản phẩm",
                CustomerName = form.CustomerName,
                CustomerPhone = form.CustomerPhone,
                IssueDescription = form.IssueDescription,
                ClaimStatus = WarrantyStatus.Received,
            };

            _context.WarrantyClaims.Add(entity);
            await _context.SaveChangesAsync();
            return new ReturnRequestResult(entity.Id, entity.Code);
        }

        public Task<DataTableJson> GetAllAsync(BaseQuery query)
        {
            EnsureManager();
            return BuildPaged(query, _context.WarrantyClaims);
        }

        public Task<DataTableJson> GetMyAsync(BaseQuery query)
        {
            var userId = _requestContext.GetUserId();
            return BuildPaged(query, _context.WarrantyClaims.Where(w => w.UserId == userId))
                ;
        }

        public async Task UpdateStatusAsync(Guid id, WarrantyStatus status, string? note)
        {
            EnsureManager();
            var entity = await _context.WarrantyClaims.FirstOrDefaultAsync(w => w.Id == id)
                ?? throw new NotFoundException("Không tìm thấy phiếu bảo hành");

            entity.ClaimStatus = status;
            if (note != null) entity.Note = note;
            if (status == WarrantyStatus.Completed) entity.CompletedAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        private async Task<DataTableJson> BuildPaged(BaseQuery query, IQueryable<WarrantyClaim> source)
        {
            var filtered = source
                .ApplyQuery(query)
                .WithDynamicSearch()
                .WithDateFilter(x => x.Created)
                .WithSort("Created")
                .GetQuery();

            var recordsTotal = await source.CountAsync();
            var recordsFiltered = await filtered.CountAsync();
            var data = await filtered.Paginate(query).Select(w => new WarrantyVM
            {
                Id = w.Id,
                Code = w.Code,
                OrderCode = w.Order!.OrderCode,
                ProductName = w.ProductName,
                CustomerName = w.CustomerName,
                CustomerPhone = w.CustomerPhone,
                IssueDescription = w.IssueDescription,
                ClaimStatus = w.ClaimStatus,
                Note = w.Note,
                Created = w.Created,
                CompletedAt = w.CompletedAt,
            }).ToListAsync();

            return new DataTableJson { recordsTotal = recordsTotal, recordsFiltered = recordsFiltered, data = data };
        }

        private bool IsManager() => _requestContext.IsSuperUser() || _requestContext.HasPermission("ORDER_MANAGE");
        private void EnsureManager()
        {
            if (!IsManager()) throw new UnauthorizedAccessException("Bạn không có quyền quản lý bảo hành");
        }
    }
}
