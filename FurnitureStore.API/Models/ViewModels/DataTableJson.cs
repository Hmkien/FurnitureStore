namespace FurnitureStore.API.Models.ViewModels
{
    public class DataTableJson
    {
        public int? draw { get; set; }

        public int? recordsTotal { get; set; }
        public int? recordsFiltered { get; set; }
        public object? data { get; set; }
        public string? exMessage { get; set; }
        public string? querytext { get; set; }
    }

    /// <summary>
    /// Generic version của DataTableJson cho việc query phân trang
    /// </summary>
    public class DataTableJson<T> where T : class
    {
        public List<T> Data { get; set; } = new();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    /// <summary>
    /// Response wrapper cho API với phân trang
    /// </summary>
    public class PagedResponse<T> where T : class
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
        public List<T> Data { get; set; } = new();
        public PaginationMetadata Pagination { get; set; } = new();
    }

    /// <summary>
    /// Metadata cho phân trang
    /// </summary>
    public class PaginationMetadata
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }
}

