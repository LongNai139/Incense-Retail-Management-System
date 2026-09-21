using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.Models.Mappers;
using SV22T1080045.Shop.Models.ViewModels.Product;

namespace SV22T1080045.Shop.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public IActionResult Index(
            string searchValue = "",
            int? categoryId = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            string? origin = null,
            string? usageTag = null,
            int? minRating = null,
            string sortBy = "default",
            int page = 1)
        {
            const int pageSize = 12;
            var currentPage = Math.Max(1, page);

            var result = _productService.ListProductsFiltered(
                ProductQueryMappingExtensions.ToProductQuery(
                    searchValue,
                    categoryId,
                    priceMin,
                    priceMax,
                    origin,
                    usageTag,
                    minRating,
                    sortBy,
                    currentPage,
                    pageSize));

            var categories = _categoryService.ListCategories();

            var model = new ProductListViewModel
            {
                SearchValue = searchValue,
                CategoryId = categoryId,
                PriceMin = priceMin,
                PriceMax = priceMax,
                Origin = origin,
                UsageTag = usageTag,
                MinRating = minRating,
                SortBy = sortBy,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = result.TotalCount,
                OriginOptions = result.OriginOptions,
                OriginCounts = result.OriginCounts,
                Categories = categories
                    .Select(c => c.ToCategoryFilterItem(result.CategoryCounts.GetValueOrDefault(c.Id, 0)))
                    .ToList(),
                Products = result.Products
                    .Select(p => p.ToCardViewModel())
                    .ToList()
            };

            return View(model);
        }
    }
}
