namespace FurnitureStore.API.Models.ViewModels
{
    public class ReviewVM
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime Created { get; set; }
    }

    public class ProductReviewSummaryVM
    {
        public Guid ProductId { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<ReviewVM> Reviews { get; set; } = new();
    }
}
