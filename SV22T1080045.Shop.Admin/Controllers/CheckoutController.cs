using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Models.Requests.Checkout;

namespace SV22T1080045.Shop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ICartService _cartService;
        private readonly IGuestOrderService _guestOrderService;
        private readonly IOrderService _orderService;
        private readonly IVoucherService _voucherService;

        public CheckoutController(
            IOrderService orderService,
            ICartService cartService,
            IGuestOrderService guestOrderService,
            IVoucherService voucherService,
            IAccountService accountService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _guestOrderService = guestOrderService;
            _voucherService = voucherService;
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = _cartService.GetCart();
            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(CheckoutInput input)
        {
            if (!ModelState.IsValid)
                return View("Index", _cartService.GetCart());

            var cart = _cartService.GetCart();
            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            var customerId = 0;
            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim != null)
                int.TryParse(customerIdClaim.Value, out customerId);

            var voucherApplied = false;
            if (!string.IsNullOrWhiteSpace(input.VoucherCode))
            {
                var voucherResult = _voucherService.Apply(input.VoucherCode, cart.Sum(i => i.TotalPrice));
                voucherApplied = voucherResult.Success;
            }

            var orderId = _orderService.InitOrder(
                input.ShippingName,
                input.ShippingPhone,
                input.ShippingAddress,
                cart,
                customerId);

            if (orderId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Đặt hàng thất bại, vui lòng thử lại.");
                return View("Index", cart);
            }

            if (voucherApplied)
                _voucherService.Use(input.VoucherCode!);

            var isGuest = customerId == 0;
            if (isGuest)
                _guestOrderService.SaveGuestOrder(orderId, input.ShippingPhone);

            _cartService.ClearCart();

            return RedirectToAction("Success", new { orderId, isGuest });
        }

        [HttpGet]
        public IActionResult Success(int orderId, bool isGuest = false)
        {
            var order = _orderService.GetOrder(orderId);
            if (order == null)
                return NotFound();

            ViewBag.IsGuest = isGuest;
            ViewBag.ShowRegisterPrompt = isGuest;
            ViewBag.OrderDetails = _orderService.GetOrderDetails(orderId);
            ViewBag.OrderID = order.Id;
            return View(order);
        }

        [HttpPost]
        public IActionResult ApplyVoucher([FromBody] ApplyVoucherRequest req)
        {
            var result = _voucherService.Apply(req.Code, req.OrderAmount);
            return Json(new
            {
                success = result.Success,
                message = result.Message,
                discount = result.DiscountAmount
            });
        }

        [HttpPost]
        public IActionResult QuickRegister([FromBody] QuickRegisterRequest req)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu đăng ký nhanh không hợp lệ." });

            if (string.IsNullOrWhiteSpace(req.Phone) || string.IsNullOrWhiteSpace(req.Password))
                return Json(new { success = false, message = "Số điện thoại và mật khẩu là bắt buộc." });

            var order = _orderService.GetOrder(req.OrderId);
            if (order == null)
                return Json(new { success = false, message = "Đơn hàng không tồn tại." });

            if (!string.Equals(order.ShippingPhone?.Trim(), req.Phone.Trim(), StringComparison.Ordinal))
                return Json(new { success = false, message = "Số điện thoại không khớp với đơn hàng." });

            var customer = new Customer
            {
                CustomerName = string.IsNullOrWhiteSpace(req.CustomerName) ? order.ShippingName : req.CustomerName,
                Phone = req.Phone,
                Password = req.Password,
                Role = "Customer",
                Address = order.ShippingAddress
            };

            if (!_accountService.Register(customer))
                return Json(new { success = false, message = "Số điện thoại đã tồn tại hoặc đăng ký thất bại." });

            _guestOrderService.MergeToCustomer(req.Phone, customer.Id);
            return Json(new { success = true, message = "Tài khoản đã được tạo thành công." });
        }
    }
}
