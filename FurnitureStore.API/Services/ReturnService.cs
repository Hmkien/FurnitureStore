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
    public class ReturnService : IReturnService
    {
        private readonly ApplicationDbContext _context;
        private readonly RequestContext _requestContext;

        public ReturnService(ApplicationDbContext context, RequestContext requestContext)
        {
            _context = context;
            _requestContext = requestContext;
        }

        public async Task<ReturnRequestResult> CreateAsync(CreateReturnForm form)
        {
            var userId = _requestContext.GetUserId();
            if (form.Items.Count == 0)
                throw new BadRequestException("Chọn ít nhất 1 sản phẩm");

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == form.OrderId)
                ?? throw new NotFoundException("Không tìm thấy đơn hàng");
            if (order.UserId != userId && !IsManager())
                throw new UnauthorizedAccessException("Không có quyền tạo yêu cầu cho đơn này");

            var variantIds = form.Items.Select(i => i.VariantId).ToList();
            var variants = await _context.ProductVariants
                .Include(v => v.Product)
                .Where(v => variantIds.Contains(v.Id))
                .ToListAsync();

            var entity = new ReturnRequest
            {
                Code = $"RT{DateTime.Now:yyMMddHHmmss}",
                OrderId = order.Id,
                UserId = userId,
                Type = form.Type,
                Reason = form.Reason,
                RequestStatus = ReturnStatus.Requested,
                Items = form.Items.Select(i => new ReturnItem
                {
                    VariantId = i.VariantId,
                    Quantity = i.Quantity,
                    ProductName = variants.FirstOrDefault(v => v.Id == i.VariantId)?.Product?.Name ?? "Sản phẩm",
                }).ToList(),
            };

            _context.ReturnRequests.Add(entity);
            await _context.SaveChangesAsync();
            return new ReturnRequestResult(entity.Id, entity.Code);
        }

        public Task<DataTableJson> GetAllAsync(BaseQuery query)
        {
            EnsureManager();
            return BuildPaged(query, _context.ReturnRequests);
        }

        public Task<DataTableJson> GetMyAsync(BaseQuery query)
        {
            var userId = _requestContext.GetUserId();
            return BuildPaged(query, _context.ReturnRequests.Where(r => r.UserId == userId))
                ;
        }

        public async Task<ReturnVM> GetDetailAsync(Guid id)
        {
            var vm = await _context.ReturnRequests.Where(r => r.Id == id).Select(Projection()).FirstOrDefaultAsync()
                ?? throw new NotFoundException("Không tìm thấy yêu cầu");
            if (vm == null) throw new NotFoundException("Không tìm thấy yêu cầu");
            return vm;
        }

        public async Task UpdateStatusAsync(Guid id, ReturnStatus status, string? note)
        {
            EnsureManager();
            var entity = await _context.ReturnRequests.Include(r => r.Items!).FirstOrDefaultAsync(r => r.Id == id)
                ?? throw new NotFoundException("Không tìm thấy yêu cầu");

            entity.RequestStatus = status;
            if (note != null) entity.Note = note;

            // Hoàn tất trả hàng/hoàn tiền → cộng lại tồn kho.
            if (status == ReturnStatus.Completed && entity.Type != ReturnType.Exchange)
            {
                var ids = entity.Items!.Select(i => i.VariantId).ToList();
                var variants = await _context.ProductVariants.Where(v => ids.Contains(v.Id)).ToListAsync();
                foreach (var item in entity.Items!)
                {
                    var v = variants.FirstOrDefault(x => x.Id == item.VariantId);
                    if (v != null) v.StockQuantity += item.Quantity;
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task<DataTableJson> BuildPaged(BaseQuery query, IQueryable<ReturnRequest> source)
        {
            var filtered = source
                .ApplyQuery(query)
                .WithDynamicSearch()
                .WithDateFilter(x => x.Created)
                .WithSort("Created")
                .GetQuery();

            var recordsTotal = await source.CountAsync();
            var recordsFiltered = await filtered.CountAsync();
            var data = await filtered.Paginate(query).Select(Projection()).ToListAsync();

            return new DataTableJson { recordsTotal = recordsTotal, recordsFiltered = recordsFiltered, data = data };
        }

        private static System.Linq.Expressions.Expression<Func<ReturnRequest, ReturnVM>> Projection() => r => new ReturnVM
        {
            Id = r.Id,
            Code = r.Code,
            OrderCode = r.Order!.OrderCode,
            CustomerName = r.User!.UserName,
            Type = r.Type,
            RequestStatus = r.RequestStatus,
            Reason = r.Reason,
            Note = r.Note,
            Created = r.Created,
            Items = r.Items!.Select(i => new ReturnItemVM { VariantId = i.VariantId, ProductName = i.ProductName, Quantity = i.Quantity }).ToList(),
        };

        private bool IsManager() => _requestContext.IsSuperUser() || _requestContext.HasPermission("ORDER_MANAGE");
        private void EnsureManager()
        {
            if (!IsManager()) throw new UnauthorizedAccessException("Bạn không có quyền quản lý đổi/trả");
        }
    }
}
