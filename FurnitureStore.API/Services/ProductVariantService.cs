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
    public class ProductVariantService : CrudService<ProductVariant, ProductVariantForm>, IProductVariantService
    {
        private readonly IRepository<Product> _productRepository;

        public ProductVariantService(
            IRepository<ProductVariant> repository,
            IRepository<Product> productRepository) : base(repository)
        {
            _productRepository = productRepository;
        }

        protected override StatusEntity InitialStatus => StatusEntity.Approved;
        protected override bool LockWhenApproved => false;

        public Task<List<ProductVariant>> GetByProductAsync(Guid productId)
            => Repository.Query()
                .Where(v => v.ProductId == productId)
                .ToListAsync();

        protected override async Task ValidateAsync(ProductVariantForm form, Guid? existingId = null)
        {
            if (!await _productRepository.AnyAsync(p => p.Id == form.ProductId))
                throw new NotFoundException("Không tìm thấy sản phẩm");

            if (await Repository.AnyAsync(v => v.SkuVariant == form.SkuVariant && v.Id != existingId))
                throw new ConflictException($"SKU biến thể '{form.SkuVariant}' đã tồn tại");
        }

        protected override ProductVariant MapToNew(ProductVariantForm form) => new()
        {
            ProductId = form.ProductId,
            Size = form.Size,
            Material = form.Material,
            Color = form.Color,
            Condition = form.Condition,
            Price = form.Price,
            StockQuantity = form.StockQuantity,
            SkuVariant = form.SkuVariant
        };

        protected override void ApplyUpdate(ProductVariant entity, ProductVariantForm form)
        {
            entity.Size = form.Size;
            entity.Material = form.Material;
            entity.Color = form.Color;
            entity.Condition = form.Condition;
            entity.Price = form.Price;
            entity.StockQuantity = form.StockQuantity;
            entity.SkuVariant = form.SkuVariant;
        }
    }
}
