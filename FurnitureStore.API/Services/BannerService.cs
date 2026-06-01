using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Enums;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Repositories;
using FurnitureStore.API.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Services
{
    public class BannerService : CrudService<Banner, BannerForm>, IBannerService
    {
        public BannerService(IRepository<Banner> repository) : base(repository)
        {
        }

        public Task<List<Banner>> GetActiveAsync(string? position)
        {
            var now = DateTime.Now;
            var query = Repository.Query().Where(b => b.Status == StatusEntity.Approved
                && (b.StartDate == null || b.StartDate <= now)
                && (b.EndDate == null || b.EndDate >= now));

            if (!string.IsNullOrWhiteSpace(position))
                query = query.Where(b => b.Position == position);

            return query.OrderBy(b => b.SortOrder).ToListAsync();
        }

        protected override Banner MapToNew(BannerForm form) => new()
        {
            Title = form.Title,
            ImageUrl = form.ImageUrl,
            LinkUrl = form.LinkUrl,
            Position = form.Position,
            SortOrder = form.SortOrder,
            StartDate = form.StartDate,
            EndDate = form.EndDate
        };

        protected override void ApplyUpdate(Banner entity, BannerForm form)
        {
            entity.Title = form.Title;
            entity.ImageUrl = form.ImageUrl;
            entity.LinkUrl = form.LinkUrl;
            entity.Position = form.Position;
            entity.SortOrder = form.SortOrder;
            entity.StartDate = form.StartDate;
            entity.EndDate = form.EndDate;
        }
    }
}
