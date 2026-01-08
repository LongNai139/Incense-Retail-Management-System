using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers;
using SV22T1080045.Shop.DomainModels;
using System.Security.Claims;
using System.Text.Json;

namespace SV22T1080045.Shop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly AccountService _accountService;

        public OrderController(OrderService orderService, AccountService accountService)
        {
            _orderService = orderService;
            _accountService = accountService;
        }

        private List<CartItem> GetCart()
        {
            var sessionString = HttpContext.Session.GetString("SHOP_CART");
            if (string.IsNullOrEmpty(sessionString)) return new List<CartItem>();
            return JsonSerializer.Deserialize<List<CartItem>>(sessionString) ?? new List<CartItem>();
        }

        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (cart.Count == 0) return RedirectToAction("Index", "Cart");

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _accountService.GetCustomer(userId);

            ViewBag.Cart = cart;
            return View(user);
        }

        [HttpPost] 
        public IActionResult Checkout(string deliveryAddress, string deliveryPhone)
        {
            var cart = GetCart();
            if (cart.Count == 0) return RedirectToAction("Index", "Cart");

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            int orderID = _orderService.InitOrder(userId, deliveryAddress, deliveryPhone, cart);

            if (orderID > 0)
            {
                HttpContext.Session.Remove("SHOP_CART");
                return RedirectToAction("Success", new { id = orderID });
            }
            return View();
        }

        public IActionResult Success(int id)
        {
            ViewBag.OrderID = id;
            return View();
        }
        public IActionResult History()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var model = _orderService.GetCustomerOrders(userId);
            return View(model);
        }

        public IActionResult Details(int id)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var order = _orderService.GetOrder(id);
            // Kiểm tra: Đơn hàng phải tồn tại và phải là của User đang đăng nhập
            if (order == null || order.CustomerID != userId)
            {
                return RedirectToAction("History");
            }

            var details = _orderService.GetOrderDetails(id);
            ViewBag.Order = order;
            return View(details); 
        }
    }
}