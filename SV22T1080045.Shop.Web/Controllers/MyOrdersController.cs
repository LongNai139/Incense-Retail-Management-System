using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.Abstractions;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Models.Mappers;
using SV22T1080045.Shop.Models.ViewModels.Orders;
using System.Security.Claims;

namespace SV22T1080045.Shop.Controllers
{
    public class MyOrdersController : Controller
    {
        private const string VerifiedPhoneSessionKey = "verified_phone";

        private readonly ICartService _cartService;
        private readonly IGuestOrderService _guestOrderService;
        private readonly IOrderService _orderService;

        public MyOrdersController(
            IOrderService orderService,
            IGuestOrderService guestOrderService,
            ICartService cartService)
        {
            _orderService = orderService;
            _guestOrderService = guestOrderService;
            _cartService = cartService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var customerId = GetCurrentCustomerId();
            var verifiedPhone = PhoneNumberHelper.NormalizeVietnameseMobile(HttpContext.Session.GetString(VerifiedPhoneSessionKey));
            var isAccountHistory = customerId > 0;

            List<Order> orders;
            if (isAccountHistory)
            {
                orders = _orderService.GetCustomerOrders(customerId);
            }
            else if (!string.IsNullOrWhiteSpace(verifiedPhone))
            {
                orders = _guestOrderService.GetOrdersByPhone(verifiedPhone);
            }
            else
            {
                TempData["LookupMessage"] = "Vui lòng xác thực số điện thoại để xem lịch sử đơn hàng.";
                return RedirectToAction("Index", "OrderLookup");
            }

            var model = new MyOrdersViewModel
            {
                IsGuestLookup = !isAccountHistory,
                LookupPhone = verifiedPhone,
                Orders = orders.Select(ToOrderListItem).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reorder(int id)
        {
            var order = _orderService.GetOrder(id);
            if (order == null || !CanAccessOrder(order))
                return RedirectToAction(nameof(Index));

            if (order.Status != 4)
                return RedirectToAction("Detail", "OrderTracking", new { id });

            var details = _orderService.GetOrderDetails(id);
            foreach (var item in details)
                _cartService.AddToCart(item.ProductId, item.Quantity);

            TempData["CartMessage"] = $"Đã thêm lại sản phẩm từ đơn {order.ToOrderListItem(details.Sum(d => d.Quantity), "").OrderCode}.";
            return RedirectToAction("Index", "Cart");
        }

        private OrderListItemViewModel ToOrderListItem(Order order)
        {
            var details = _orderService.GetOrderDetails(order.Id);
            var trackUrl = Url.Action("Detail", "OrderTracking", new { id = order.Id }) ?? $"/don-hang/theo-doi/{order.Id}";
            return order.ToOrderListItem(details.Sum(d => d.Quantity), trackUrl);
        }

        private bool CanAccessOrder(Order order)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId > 0 && order.CustomerId == customerId)
                return true;

            var verifiedPhone = PhoneNumberHelper.NormalizeVietnameseMobile(HttpContext.Session.GetString(VerifiedPhoneSessionKey));
            if (string.IsNullOrWhiteSpace(verifiedPhone))
                return false;

            return _guestOrderService.GetOrdersByPhone(verifiedPhone).Any(o => o.Id == order.Id);
        }

        private int GetCurrentCustomerId()
        {
            var claim = User.FindFirst("CustomerId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out var id) ? id : 0;
        }
    }
}
