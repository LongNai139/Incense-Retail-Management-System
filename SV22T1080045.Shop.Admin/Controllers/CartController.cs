//using Microsoft.AspNetCore.Mvc;
//using SV22T1080045.Shop.BusinessLayers;

//namespace SV22T1080045.Shop.Controllers
//{
//    public class CartController : Controller
//    {
//        private readonly CartService _cartService;

//        public CartController(CartService cartService)
//        {
//            _cartService = cartService;
//        }

//        // Xem giỏ hàng
//        public IActionResult Index()
//        {
//            var model = _cartService.GetCart();
//            return View(model);
//        }

//        // Thêm vào giỏ
//        public IActionResult AddToCart(int productID, int quantity = 1)
//        {
//            _cartService.AddToCart(productID, quantity);
//            return RedirectToAction("Index");
//        }

//        // Xóa 1 món
//        public IActionResult RemoveFromCart(int id)
//        {
//            _cartService.RemoveFromCart(id);
//            return RedirectToAction("Index");
//        }

//        // Cập nhật số lượng
//        [HttpPost]
//        public IActionResult UpdateQuantity(int id, int quantity)
//        {
//            _cartService.UpdateQuantity(id, quantity);
//            return RedirectToAction("Index");
//        }

//        // Xóa hết
//        public IActionResult ClearCart()
//        {
//            _cartService.ClearCart();
//            return RedirectToAction("Index");
//        }
//    }
//}