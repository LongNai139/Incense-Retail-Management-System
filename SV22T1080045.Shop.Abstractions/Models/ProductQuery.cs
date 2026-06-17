namespace SV22T1080045.Shop.Abstractions.Models
{
    public class ProductQuery
    {
        public string? SearchValue { get; set; }
        public int? CategoryId { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public string? Origin { get; set; }
        public string? UsageTag { get; set; }
        public int? MinRating { get; set; }
        public string SortBy { get; set; } = "default";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
