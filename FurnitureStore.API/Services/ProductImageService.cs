using FurnitureStore.API.Common.Exceptions;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Enums;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Repositories;
using FurnitureStore.API.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Services
{
    public class ProductImageService : CrudService<ProductImage, ProductImageForm>, IProductImageService
    {
        private readonly IRepository<Product> _productRepository;

        public ProductImageService(
            IRepository<ProductImage> repository,
            IRepository<Product> productRepository) : base(repository)
        {
            _productRepository = productRepository;
        }

        protected override StatusEntity InitialStatus => StatusEntity.Approved;
        protected override bool LockWhenApproved => false;

        public Task<List<ProductImage>> GetByProductAsync(Guid productId)
            => Repository.Query()
                .Where(i => i.ProductId == productId)
                .OrderByDescending(i => i.IsPrimary)
                .ToListAsync();

        protected override async Task ValidateAsync(ProductImageForm form, Guid? existingId = null)
        {
            if (!await _productRepository.AnyAsync(p => p.Id == form.ProductId))
                throw new NotFoundException("Không tìm thấy sản phẩm");
        }

        protected override ProductImage MapToNew(ProductImageForm form) => new()
        {
            ProductId = form.ProductId,
            ImageUrl = form.ImageUrl,
            IsPrimary = form.IsPrimary
        };

        protected override void ApplyUpdate(ProductImage entity, ProductImageForm form)
        {
            entity.ImageUrl = form.ImageUrl;
            entity.IsPrimary = form.IsPrimary;
        }
    }
}
