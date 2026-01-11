//using Microsoft.AspNetCore.Mvc;
//using SV22T1080045.Shop.BusinessLayers;

//namespace SV22T1080045.Shop.Controllers
//{
//    public class HomeController : Controller
//    {
//        private readonly ProductService _productService;

//        public HomeController(ProductService productService)
//        {
//            _productService = productService;
//        }

//        // Trang chủ: Hiển thị danh sách và Form tìm kiếm
//        public IActionResult Index(string searchValue = "", int categoryID = 0, decimal minPrice = 0, decimal maxPrice = 0)
//        {
//            var model = _productService.Search(searchValue, categoryID, minPrice, maxPrice);

//            ViewBag.Categories = _productService.GetCategories();

//            // Giữ lại giá trị tìm kiếm để hiển thị lại trên form (UX)
//            ViewBag.SearchValue = searchValue;
//            ViewBag.CategoryID = categoryID;
//            ViewBag.MinPrice = minPrice;
//            ViewBag.MaxPrice = maxPrice;

//            return View(model);
//        }

//        public IActionResult Details(int id)
//        {
//            var product = _productService.GetProduct(id);
//            if (product == null)
//            {
//                return RedirectToAction("Index");
//            }
//            return View(product);
//        }

//        public IActionResult Error() => View();
//    }
//}