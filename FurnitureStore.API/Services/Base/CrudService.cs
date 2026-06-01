using FurnitureStore.API.Common.Exceptions;
using FurnitureStore.API.Extensions;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Enums;
using FurnitureStore.API.Models.Query;
using FurnitureStore.API.Models.ViewModels;
using FurnitureStore.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Services.Base
{
    /// <summary>
    /// Service CRUD chung cho các entity có vòng đời Approve/Reject giống nhau
    /// (Role, Permision...). Lớp con chỉ cần khai báo cách map form -> entity.
    /// </summary>
    public abstract class CrudService<TEntity, TForm> : ICrudService<TEntity, TForm> where TEntity : BaseEntity
    {
        protected readonly IRepository<TEntity> Repository;

        protected CrudService(IRepository<TEntity> repository)
        {
            Repository = repository;
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await Repository.GetByIdAsync(id)
                ?? throw new NotFoundException();
        }

        public async Task<TEntity> Create(TForm form)
        {
            await ValidateAsync(form);

            var entity = MapToNew(form);
            entity.Status = InitialStatus;

            await Repository.AddAsync(entity);
            await Repository.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Update(Guid id, TForm form)
        {
            var entity = await GetByIdAsync(id);
            EnsureEditable(entity);
            await ValidateAsync(form, id);
            ApplyUpdate(entity, form);

            Repository.Update(entity);
            return await Repository.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(Guid id)
        {
            var entity = await GetByIdAsync(id);
            EnsureEditable(entity);

            Repository.Remove(entity);
            return await Repository.SaveChangesAsync() > 0;
        }

        public Task<bool> Approved(Guid id) => SetStatus(id, StatusEntity.Approved);

        public Task<bool> Reject(Guid id) => SetStatus(id, StatusEntity.Rejected);

        /// <summary>Trạng thái khi vừa tạo. Mặc định Pending (chờ duyệt) để có thể sửa.</summary>
        protected virtual StatusEntity InitialStatus => StatusEntity.Pending;

        /// <summary>Khi true: bản ghi đã duyệt (Approved) bị khóa sửa/xóa, phải hủy duyệt trước.</summary>
        protected virtual bool LockWhenApproved => true;

        private void EnsureEditable(TEntity entity)
        {
            if (LockWhenApproved && entity.Status == StatusEntity.Approved)
                throw new BadRequestException("Bản ghi đã duyệt — hãy hủy duyệt trước khi sửa/xóa");
        }

        public virtual async Task<DataTableJson> GetPaged(BaseQuery query)
        {
            var filtered = ApplyFilters(Repository.Query()
                    .ApplyQuery(query)
                    .WithDynamicSearch()
                    .WithDateFilter(x => x.Created), query)
                .WithSort("Created")
                .GetQuery();

            var recordsTotal = await Repository.Query().CountAsync();
            var recordsFiltered = await filtered.CountAsync();
            var data = await filtered.Paginate(query).ToListAsync();

            return new DataTableJson
            {
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        private async Task<bool> SetStatus(Guid id, StatusEntity status)
        {
            var entity = await GetByIdAsync(id);
            entity.Status = status;

            Repository.Update(entity);
            return await Repository.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Hook để lớp con thêm filter đặc thù (theo giá, danh mục...).
        /// Mặc định không lọc thêm gì.
        /// </summary>
        protected virtual QueryBuilder<TEntity> ApplyFilters(QueryBuilder<TEntity> builder, BaseQuery query)
            => builder;

        /// <summary>
        /// Kiểm tra hợp lệ trước khi Create/Update (vd: trùng slug/SKU).
        /// <paramref name="existingId"/> khác null khi đang cập nhật.
        /// Ném <see cref="ConflictException"/> nếu vi phạm.
        /// </summary>
        protected virtual Task ValidateAsync(TForm form, Guid? existingId = null) => Task.CompletedTask;

        /// <summary>Tạo entity mới từ form (lớp con cài đặt).</summary>
        protected abstract TEntity MapToNew(TForm form);

        /// <summary>Áp dụng thay đổi từ form lên entity đã tồn tại (lớp con cài đặt).</summary>
        protected abstract void ApplyUpdate(TEntity entity, TForm form);
    }
}
