using FurnitureStore.API.Data;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<NotificationVM>> GetRecentAsync(int take)
        {
            take = Math.Clamp(take, 1, 50);

            var orders = await _context.Orders
                .OrderByDescending(o => o.Created)
                .Take(take)
                .Select(o => new NotificationVM
                {
                    Type = "order",
                    Title = $"Đơn hàng mới {o.OrderCode}",
                    Description = $"{o.ReceiverName} · {o.TotalAmount:#,##0} đ",
                    CreatedAt = o.Created,
                    Link = $"/orders/{o.Id}",
                })
                .ToListAsync();

            var reviews = await _context.Reviews
                .OrderByDescending(r => r.Created)
                .Take(take)
                .Select(r => new NotificationVM
                {
                    Type = "review",
                    Title = $"Đánh giá {r.Rating}★ mới",
                    Description = $"{r.User!.UserName} · {r.Product!.Name}",
                    CreatedAt = r.Created,
                    Link = null,
                })
                .ToListAsync();

            var returns = await _context.ReturnRequests
                .OrderByDescending(r => r.Created)
                .Take(take)
                .Select(r => new NotificationVM
                {
                    Type = "return",
                    Title = $"Yêu cầu đổi/trả {r.Code}",
                    Description = $"{r.User!.UserName} · đơn {r.Order!.OrderCode}",
                    CreatedAt = r.Created,
                    Link = null,
                })
                .ToListAsync();

            var warranties = await _context.WarrantyClaims
                .OrderByDescending(w => w.Created)
                .Take(take)
                .Select(w => new NotificationVM
                {
                    Type = "warranty",
                    Title = $"Yêu cầu bảo hành {w.Code}",
                    Description = $"{w.CustomerName} · {w.ProductName}",
                    CreatedAt = w.Created,
                    Link = null,
                })
                .ToListAsync();

            return orders
                .Concat(reviews)
                .Concat(returns)
                .Concat(warranties)
                .OrderByDescending(n => n.CreatedAt)
                .Take(take)
                .ToList();
        }
    }
}
