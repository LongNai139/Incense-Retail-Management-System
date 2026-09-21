using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.Models.Mappers;
using SV22T1080045.Shop.Models.ViewModels.Home;

namespace SV22T1080045.Shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public IActionResult Index(string searchValue = "")
        {
            var model = new HomeViewModel
            {
                FeaturedProducts = _productService
                    .ListProducts(searchValue, take: 4, sortBy: "bestseller")
                    .Select(p => p.ToCardViewModel())
                    .ToList(),
                LatestProducts = _productService
                    .ListProducts(searchValue, take: 4, sortBy: "newest")
                    .Select(p => p.ToCardViewModel())
                    .ToList(),
                Categories = _categoryService.ListCategories(6)
                    .Select(c => c.ToCategoryViewModel())
                    .ToList()
            };

            ViewBag.SearchValue = searchValue;
            return View(model);
        }

        public IActionResult Details(int id)
        {
            var product = _productService.GetProduct(id);
            if (product == null)
                return RedirectToAction(nameof(Index));

            var relatedProducts = _productService.ListRelatedProducts(id, product.CategoryId, 4);
            return View(product.ToDetailsViewModel(relatedProducts));
        }

        public IActionResult Privacy() => View();

        public IActionResult Error() => View();
    }
}
