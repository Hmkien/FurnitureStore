namespace FurnitureStore.API.Models.Form
{
    /// <summary>Dữ liệu multipart cho upload media.</summary>
    public class MediaUploadForm
    {
        public IFormFile File { get; set; } = default!;
        public string? Folder { get; set; }
    }
}
