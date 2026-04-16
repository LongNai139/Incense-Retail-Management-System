using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers;
using SV22T1080045.Shop.DomainModels;
using System.ComponentModel.DataAnnotations;

namespace SV22T1080045.Shop.Admin.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IGuestOrderService _guestOrderService;
        private readonly IVoucherService _voucherService;
        private readonly IOtpService _otpService;

        public CheckoutController(
            IOrderService orderService,
            ICartService cartService,
            IGuestOrderService guestOrderService,
            IVoucherService voucherService,
            IOtpService otpService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _guestOrderService = guestOrderService;
            _voucherService = voucherService;
            _otpService = otpService;
        }

        // GET /Checkout ── Trang thanh toán
        [HttpGet]
        public IActionResult Index()
        {
            var cart = _cartService.GetCart();
            if (!cart.Any()) return RedirectToAction("Index", "Cart");
            return View(cart);
        }

        // POST /Checkout/PlaceOrder ── Đặt hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(CheckoutInput input)
        {
            if (!ModelState.IsValid)
                return View("Index", _cartService.GetCart());

            var cart = _cartService.GetCart();
            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            // Xác định CustomerId (0 nếu guest chưa đăng nhập)
            int customerId = 0;
            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim != null)
                int.TryParse(customerIdClaim.Value, out customerId);

            // Áp mã giảm giá nếu có
            if (!string.IsNullOrWhiteSpace(input.VoucherCode))
            {
                var vResult = _voucherService.Apply(input.VoucherCode, cart.Sum(i => i.TotalPrice));
                if (vResult.Success)
                    _voucherService.Use(input.VoucherCode);
            }

            // Tạo đơn hàng — gọi đúng method InitOrder của bạn
            int orderId = _orderService.InitOrder(
                shippingName: input.ShippingName,
                shippingPhone: input.ShippingPhone,
                shippingAddress: input.ShippingAddress,
                cart: cart,
                customerId: customerId
            );

            if (orderId <= 0)
            {
                ModelState.AddModelError("", "Đặt hàng thất bại, vui lòng thử lại.");
                return View("Index", cart);
            }

            // Lưu GuestOrder nếu là khách vãng lai
            bool isGuest = customerId == 0;
            if (isGuest)
                _guestOrderService.SaveGuestOrder(orderId, input.ShippingPhone);

            // Xóa giỏ hàng
            _cartService.ClearCart();

            return RedirectToAction("Success", new { orderId, isGuest });
        }

        // GET /Checkout/Success
        [HttpGet]
        public IActionResult Success(int orderId, bool isGuest = false)
        {
            var order = _orderService.GetOrder(orderId);
            if (order == null) return NotFound();

            ViewBag.IsGuest = isGuest;
            ViewBag.ShowRegisterPrompt = isGuest;
            ViewBag.OrderDetails = _orderService.GetOrderDetails(orderId);
            ViewBag.OrderID = order.Id;
            return View(order);
        }

        // POST /Checkout/ApplyVoucher ── AJAX
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

        // POST /Checkout/QuickRegister ── Tạo TK nhanh sau khi mua xong
        [HttpPost]
        public IActionResult QuickRegister([FromBody] QuickRegisterRequest req)
        {
            // TODO: gọi AccountService.Register(req.Phone, req.Password)
            // sau khi tạo xong: gọi _guestOrderService.MergeToCustomer(req.Phone, newCustomerId)
            // Hiện tại trả về success để test giao diện
            return Json(new { success = true, message = "Tài khoản đã được tạo thành công!" });
        }
    }

    // ── Input Models ─────────────────────────────────────────────────────────
    public class CheckoutInput
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string ShippingName { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^(0|\+84)[3-9]\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string ShippingPhone { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string ShippingAddress { get; set; } = "";

        public string? Note { get; set; }
        public string? VoucherCode { get; set; }
        public int PaymentMethod { get; set; } = 1; // 1=COD, 2=VNPay, 3=MoMo
    }

    public record ApplyVoucherRequest(string Code, decimal OrderAmount);
    public record QuickRegisterRequest(string Phone, string Password, int OrderId);
}
