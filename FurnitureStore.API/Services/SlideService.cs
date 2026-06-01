using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Enums;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Repositories;
using FurnitureStore.API.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Services
{
    public class SlideService : CrudService<Slide, SlideForm>, ISlideService
    {
        public SlideService(IRepository<Slide> repository) : base(repository)
        {
        }

        public Task<List<Slide>> GetActiveAsync()
            => Repository.Query()
                .Where(s => s.Status == StatusEntity.Approved)
                .OrderBy(s => s.SortOrder)
                .ToListAsync();

        protected override Slide MapToNew(SlideForm form) => new()
        {
            Title = form.Title,
            ImageUrl = form.ImageUrl,
            LinkUrl = form.LinkUrl,
            Caption = form.Caption,
            SortOrder = form.SortOrder
        };

        protected override void ApplyUpdate(Slide entity, SlideForm form)
        {
            entity.Title = form.Title;
            entity.ImageUrl = form.ImageUrl;
            entity.LinkUrl = form.LinkUrl;
            entity.Caption = form.Caption;
            entity.SortOrder = form.SortOrder;
        }
    }
}
