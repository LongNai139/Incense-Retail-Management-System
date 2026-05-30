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

        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                ViewBag.Title = "Thêm mới sản phẩm";
                return View(new ProductEditViewModel());
            }

            ViewBag.Title = "Cập nhật sản phẩm";
            var product = _productService.GetProduct(id);
            if (product == null)
                return RedirectToAction(nameof(Index));

            return View(product.ToEditViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Edit", model);

            try
            {
                _productService.SaveProduct(model.ToSaveRequest());
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Edit", model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
