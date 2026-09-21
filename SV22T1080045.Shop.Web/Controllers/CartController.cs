using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.Models.Mappers;
using SV22T1080045.Shop.Models.ViewModels.Cart;

namespace SV22T1080045.Shop.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public CartController(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = _cartService.GetCart();
            var cartProductIds = cart.Select(i => i.ProductID).ToHashSet();
            var upsell = _productService
                .ListProducts(take: 12, sortBy: "bestseller")
                .Where(p => !cartProductIds.Contains(p.Id))
                .Take(3)
                .Select(p => p.ToUpsellViewModel())
                .ToList();

            var model = new CartPageViewModel
            {
                Items = cart.Select(i => i.ToViewModel()).ToList(),
                UpsellProducts = upsell
            };

            return View(model);
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
