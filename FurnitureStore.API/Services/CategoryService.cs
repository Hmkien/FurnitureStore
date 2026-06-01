using FurnitureStore.API.Common.Exceptions;
using FurnitureStore.API.Extensions;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Repositories;
using FurnitureStore.API.Services.Base;

namespace FurnitureStore.API.Services
{
    public class CategoryService : CrudService<Category, CategoryForm>, ICategoryService
    {
        public CategoryService(IRepository<Category> repository) : base(repository)
        {
        }

        protected override async Task ValidateAsync(CategoryForm form, Guid? existingId = null)
        {
            var slug = BuildSlug(form);
            var duplicated = await Repository.AnyAsync(c => c.Slug == slug && c.Id != existingId);
            if (duplicated)
                throw new ConflictException($"Slug '{slug}' đã tồn tại");
        }

        protected override Category MapToNew(CategoryForm form) => new()
        {
            Name = form.Name,
            Slug = BuildSlug(form),
            Description = form.Description,
            ImageUrl = form.ImageUrl,
            ParentId = form.ParentId
        };

        protected override void ApplyUpdate(Category entity, CategoryForm form)
        {
            entity.Name = form.Name;
            entity.Slug = BuildSlug(form);
            entity.Description = form.Description;
            entity.ImageUrl = form.ImageUrl;
            entity.ParentId = form.ParentId;
        }

        private static string BuildSlug(CategoryForm form)
            => string.IsNullOrWhiteSpace(form.Slug) ? form.Name.ToSlug() : form.Slug.ToSlug();
    }
}
