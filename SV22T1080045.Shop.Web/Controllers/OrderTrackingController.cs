using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.Abstractions;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Models;
using SV22T1080045.Shop.Models.Mappers;
using System.Security.Claims;

namespace SV22T1080045.Shop.Controllers
{
    public class OrderTrackingController : Controller
    {
        private const string VerifiedPhoneSessionKey = "verified_phone";

        private readonly IOrderService _orderService;
        private readonly IGuestOrderService _guestOrderService;

        public OrderTrackingController(IOrderService orderService, IGuestOrderService guestOrderService)
        {
            _orderService = orderService;
            _guestOrderService = guestOrderService;
        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
            var order = _orderService.GetOrder(id);
            if (order == null)
                return NotFound();

            if (!CanViewOrder(order))
                return RedirectToGuestLookup();

            var model = order.ToOrderTrackViewModel(_orderService.GetOrderDetails(id));
            var customerId = GetCurrentCustomerId();
            ViewBag.HasCustomerId = customerId > 0 && order.CustomerId == customerId;
            return View(model);
        }

        [HttpGet]
        public IActionResult StatusJson(int id)
        {
            var order = _orderService.GetOrder(id);
            if (order == null || !CanViewOrder(order))
                return Json(new { error = "Không có quyền xem đơn này." });

            return Json(new
            {
                status = order.Status,
                statusText = OrderStatusLabels.GetStatusText(order.Status),
                paymentStatus = order.PaymentStatus,
                paymentStatusText = OrderStatusLabels.GetPaymentStatusText(order.PaymentStatus),
                isCancelled = order.Status == -1,
                isCompleted = order.Status == 4,
                canPoll = order.Status is not (4 or -1),
                timeline = OrderStatusLabels.BuildTimeline(order.Status)
            });
        }

        private bool CanViewOrder(Order order)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId > 0 && order.CustomerId == customerId)
                return true;

            var verifiedPhone = HttpContext.Session.GetString(VerifiedPhoneSessionKey);
            if (string.IsNullOrWhiteSpace(verifiedPhone))
                return false;

            if (PhonesMatch(verifiedPhone, order.ShippingPhone))
                return true;

            return _guestOrderService.GetOrdersByPhone(verifiedPhone).Any(o => o.Id == order.Id);
        }

        private int GetCurrentCustomerId()
        {
            var claim = User.FindFirst("CustomerId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out var id) ? id : 0;
        }

        private static bool PhonesMatch(string a, string? b)
        {
            return PhoneNumberHelper.NormalizeVietnameseMobile(a) == PhoneNumberHelper.NormalizeVietnameseMobile(b);
        }

        private IActionResult RedirectToGuestLookup()
        {
            TempData["LookupMessage"] = "Vui lòng xác thực số điện thoại để xem đơn hàng.";
            return RedirectToAction("Index", "OrderLookup");
        }
    }
}
