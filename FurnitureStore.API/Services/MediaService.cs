using FurnitureStore.API.Common.Exceptions;
using FurnitureStore.API.Extensions;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Query;
using FurnitureStore.API.Models.ViewModels;
using FurnitureStore.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Services
{
    public class MediaService : IMediaService
    {
        private static readonly string[] AllowedExtensions =
            { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".bmp" };

        private readonly IRepository<MediaFile> _repository;
        private readonly IWebHostEnvironment _env;

        public MediaService(IRepository<MediaFile> repository, IWebHostEnvironment env)
        {
            _repository = repository;
            _env = env;
        }

        public async Task<MediaFile> UploadAsync(IFormFile file, string? folder)
        {
            if (file == null || file.Length == 0)
                throw new BadRequestException("File rỗng");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new BadRequestException("Định dạng ảnh không được hỗ trợ");

            var safeFolder = string.IsNullOrWhiteSpace(folder) ? "general" : folder.ToSlug();
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var relativeDir = Path.Combine("uploads", "media", safeFolder);
            var absoluteDir = Path.Combine(webRoot, relativeDir);
            Directory.CreateDirectory(absoluteDir);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var absolutePath = Path.Combine(absoluteDir, fileName);

            await using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url = "/" + Path.Combine(relativeDir, fileName).Replace('\\', '/');

            var media = new MediaFile
            {
                FileName = file.FileName,
                Url = url,
                ContentType = file.ContentType,
                Size = file.Length,
                Folder = safeFolder
            };

            await _repository.AddAsync(media);
            await _repository.SaveChangesAsync();
            return media;
        }

        public async Task<DataTableJson> GetPaged(BaseQuery query)
        {
            var filtered = _repository.Query()
                .ApplyQuery(query)
                .WithDynamicSearch()
                .WithDateFilter(x => x.Created)
                .WithSort("Created")
                .GetQuery();

            var recordsTotal = await _repository.Query().CountAsync();
            var recordsFiltered = await filtered.CountAsync();
            var data = await filtered.Paginate(query).ToListAsync();

            return new DataTableJson
            {
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var media = await _repository.GetByIdAsync(id);
            if (media == null) return false;

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var absolutePath = Path.Combine(webRoot, media.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            try
            {
                if (File.Exists(absolutePath)) File.Delete(absolutePath);
            }
            catch
            {
                // Bỏ qua lỗi xóa file vật lý, vẫn xóa bản ghi.
            }

            _repository.Remove(media);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
