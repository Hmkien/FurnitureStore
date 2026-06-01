using FurnitureStore.API.Common.Exceptions;
using FurnitureStore.API.Extensions;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Repositories;
using FurnitureStore.API.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Services
{
    public class PostService : CrudService<Post, PostForm>, IPostService
    {
        public PostService(IRepository<Post> repository) : base(repository)
        {
        }

        public async Task<Post> GetBySlugAsync(string slug)
            => await Repository.Query().FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished)
                ?? throw new NotFoundException("Không tìm thấy bài viết");

        protected override async Task ValidateAsync(PostForm form, Guid? existingId = null)
        {
            var slug = BuildSlug(form);
            if (await Repository.AnyAsync(p => p.Slug == slug && p.Id != existingId))
                throw new ConflictException($"Slug '{slug}' đã tồn tại");
        }

        protected override Post MapToNew(PostForm form) => new()
        {
            Title = form.Title,
            Slug = BuildSlug(form),
            Summary = form.Summary,
            Content = form.Content,
            ThumbnailUrl = form.ThumbnailUrl,
            Category = form.Category,
            Author = form.Author,
            IsPublished = form.IsPublished,
            PublishedAt = form.IsPublished ? DateTime.Now : null
        };

        protected override void ApplyUpdate(Post entity, PostForm form)
        {
            entity.Title = form.Title;
            entity.Slug = BuildSlug(form);
            entity.Summary = form.Summary;
            entity.Content = form.Content;
            entity.ThumbnailUrl = form.ThumbnailUrl;
            entity.Category = form.Category;
            entity.Author = form.Author;
            if (form.IsPublished && !entity.IsPublished)
                entity.PublishedAt = DateTime.Now;
            entity.IsPublished = form.IsPublished;
        }

        private static string BuildSlug(PostForm form)
            => string.IsNullOrWhiteSpace(form.Slug) ? form.Title.ToSlug() : form.Slug.ToSlug();
    }
}
