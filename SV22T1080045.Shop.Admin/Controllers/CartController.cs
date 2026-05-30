using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers.Interfaces;

namespace SV22T1080045.Shop.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_cartService.GetCart());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int productID, int quantity = 1, bool redirectToCheckout = false)
        {
            _cartService.AddToCart(productID, quantity);
            if (redirectToCheckout)
                return RedirectToAction("Index", "Checkout");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCartAjax(int productID, int quantity = 1)
        {
            if (quantity <= 0)
                return Json(new { success = false, message = "Số lượng không hợp lệ." });

            _cartService.AddToCart(productID, quantity);
            return Json(new
            {
                success = true,
                message = "Đã thêm sản phẩm vào giỏ hàng.",
                cartCount = _cartService.Count()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int productID, int quantity)
        {
            _cartService.UpdateQuantity(productID, quantity);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int productID)
        {
            _cartService.RemoveFromCart(productID);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearCart()
        {
            _cartService.ClearCart();
            return RedirectToAction(nameof(Index));
        }
    }
}
