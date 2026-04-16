using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers;

namespace SV22T1080045.Shop.Admin.Controllers
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
            var products = _productService
                .ListProducts(searchValue)
                .Where(p => !p.IsDeleted)
                .ToList();

            ViewBag.SearchValue = searchValue;
            ViewBag.FeaturedProducts = products.Take(4).ToList();
            ViewBag.LatestProducts = products
                .OrderByDescending(p => p.CreatedTime)
                .Take(4)
                .ToList();
            ViewBag.Categories = _categoryService.ListCategories(6);

            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _productService.GetProduct(id);
            if (product == null || product.IsDeleted)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.RelatedProducts = _productService
                .ListProducts()
                .Where(p => !p.IsDeleted && p.Id != id && p.CategoryId == product.CategoryId)
                .Take(4)
                .ToList();

            return View(product);
        }

        public IActionResult Privacy() => View();

        public IActionResult Error() => View();
    }
}
