using SV22T1080045.Shop.Abstractions.Models;

namespace SV22T1080045.Shop.Models.Mappers
{
    public static class ProductQueryMappingExtensions
    {
        public static ProductQuery ToProductQuery(
            string? searchValue,
            int? categoryId,
            decimal? priceMin,
            decimal? priceMax,
            string? origin,
            string? usageTag,
            int? minRating,
            string sortBy,
            int page,
            int pageSize)
        {
            return new ProductQuery
            {
                SearchValue = searchValue,
                CategoryId = categoryId,
                PriceMin = priceMin,
                PriceMax = priceMax,
                Origin = origin,
                UsageTag = usageTag,
                MinRating = minRating,
                SortBy = sortBy,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
