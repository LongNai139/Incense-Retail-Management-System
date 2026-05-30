using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.Models.Mappers;
using SV22T1080045.Shop.Models.Requests.Checkout;
using SV22T1080045.Shop.Models.ViewModels.Cart;
using SV22T1080045.Shop.Models.ViewModels.Checkout;
using SV22T1080045.Shop.Payments.MoMo;
using SV22T1080045.Shop.Payments.VietQr;
using SV22T1080045.Shop.Payments.VnPay;
using System.Security.Claims;

namespace SV22T1080045.Shop.Controllers
{
    public class CheckoutController : Controller
    {
        private const string LastShippingAddressSessionKeyPrefix = "CHECKOUT_LAST_SHIPPING_ADDRESS_";

        private readonly IAccountService _accountService;
        private readonly ICartService _cartService;
        private readonly IGuestOrderService _guestOrderService;
        private readonly IOrderService _orderService;
        private readonly IVoucherService _voucherService;
        private readonly IVnPayService _vnPayService;
        private readonly IMoMoService _moMoService;
        private readonly IVietQrService _vietQrService;

        public CheckoutController(
            IOrderService orderService,
            ICartService cartService,
            IGuestOrderService guestOrderService,
            IVoucherService voucherService,
            IAccountService accountService,
            IVnPayService vnPayService,
            IMoMoService moMoService,
            IVietQrService vietQrService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _guestOrderService = guestOrderService;
            _voucherService = voucherService;
            _accountService = accountService;
            _vnPayService = vnPayService;
            _moMoService = moMoService;
            _vietQrService = vietQrService;
        }

        [HttpGet]
        public IActionResult Index(string? voucherCode = null)
        {
            var cart = _cartService.GetCart();
            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            var input = BuildDefaultInput();
            input.VoucherCode = voucherCode?.Trim();

            ViewBag.VnPayConfigured = _vnPayService.IsConfigured;
            ViewBag.MoMoConfigured = _moMoService.IsConfigured;
            ViewBag.VietQrConfigured = _vietQrService.IsConfigured;

            return View(BuildViewModel(cart, input));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutInput input, CancellationToken cancellationToken)
        {
            var cart = _cartService.GetCart();
            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            if (!ModelState.IsValid)
            {
                SetPaymentConfigFlags();
                return View("Index", BuildViewModel(cart, input));
            }

            if (!IsPaymentMethodSupported(input.PaymentMethod, out var paymentError))
            {
                ModelState.AddModelError(nameof(input.PaymentMethod), paymentError);
                SetPaymentConfigFlags();
                return View("Index", BuildViewModel(cart, input));
            }

            var customerId = GetCurrentCustomerId();

            var orderId = _orderService.InitOrder(
                input.ShippingName,
                input.ShippingPhone,
                input.ShippingAddress,
                cart,
                customerId,
                input.PaymentMethod,
                input.Note,
                input.VoucherCode);

            if (orderId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Đặt hàng thất bại, vui lòng thử lại.");
                SetPaymentConfigFlags();
                return View("Index", BuildViewModel(cart, input));
            }

            RememberShippingAddress(input.ShippingAddress, customerId);

            if (customerId == 0)
                _guestOrderService.SaveGuestOrder(orderId, input.ShippingPhone);

            var order = _orderService.GetOrder(orderId);
            var payableAmount = order?.FinalAmount ?? cart.Sum(i => i.TotalPrice);
            var isGuest = customerId == 0;

            if (input.PaymentMethod == 2)
            {
                var returnUrl = Url.Action(nameof(VnPayReturn), "Checkout", null, Request.Scheme)
                    ?? $"{Request.Scheme}://{Request.Host}/Checkout/VnPayReturn";

                var paymentUrl = _vnPayService.CreatePaymentUrl(
                    orderId,
                    payableAmount,
                    $"Thanh toan don hang TH{orderId:D6}",
                    GetClientIpAddress(),
                    returnUrl);

                _cartService.ClearCart();
                return Redirect(paymentUrl);
            }

            if (input.PaymentMethod == 3)
            {
                var redirectUrl = Url.Action(nameof(MoMoReturn), "Checkout", null, Request.Scheme)
                    ?? $"{Request.Scheme}://{Request.Host}/Checkout/MoMoReturn";
                var ipnUrl = Url.Action(nameof(MoMoIpn), "Checkout", null, Request.Scheme)
                    ?? $"{Request.Scheme}://{Request.Host}/Checkout/MoMoIpn";

                var momo = await _moMoService.CreatePaymentAsync(
                    orderId,
                    payableAmount,
                    $"Thanh toan TH{orderId:D6}",
                    redirectUrl,
                    ipnUrl,
                    cancellationToken);

                if (!momo.IsSuccess || string.IsNullOrWhiteSpace(momo.PayUrl))
                {
                    ModelState.AddModelError(string.Empty, momo.Message);
                    SetPaymentConfigFlags();
                    return View("Index", BuildViewModel(cart, input));
                }

                _cartService.ClearCart();
                return Redirect(momo.PayUrl);
            }

            if (input.PaymentMethod == 4)
            {
                var qr = _vietQrService.BuildPaymentInfo(new VietQrPaymentRequest
                {
                    OrderId = orderId,
                    Amount = payableAmount,
                    TransferContent = $"TH{orderId:D6}"
                });

                _cartService.ClearCart();
                return View("QrPayment", new QrPaymentViewModel
                {
                    OrderId = orderId,
                    OrderCode = qr.OrderCode,
                    QrImageUrl = qr.QrImageUrl,
                    BankId = qr.BankId,
                    AccountNumber = qr.AccountNumber,
                    AccountName = qr.AccountName,
                    Amount = qr.Amount,
                    TransferContent = qr.TransferContent,
                    IsGuest = isGuest
                });
            }

            _cartService.ClearCart();
            return RedirectToAction(nameof(Success), new { orderId, isGuest });
        }

        [HttpGet]
        public IActionResult Success(int orderId, bool isGuest = false)
        {
            var order = _orderService.GetOrder(orderId);
            if (order == null)
                return NotFound();

            var model = order.ToCheckoutSuccessViewModel(_orderService.GetOrderDetails(orderId));
            ViewBag.IsGuest = isGuest;
            ViewBag.ShowRegisterPrompt = isGuest;
            ViewBag.PaymentMessage = TempData["PaymentMessage"] as string;
            return View(model);
        }

        [HttpGet]
        public IActionResult QrPayment(int orderId, bool isGuest = false)
        {
            var order = _orderService.GetOrder(orderId);
            if (order == null || order.PaymentMethod != 4)
                return NotFound();

            if (!_vietQrService.IsConfigured)
                return RedirectToAction(nameof(Success), new { orderId, isGuest });

            var qr = _vietQrService.BuildPaymentInfo(new VietQrPaymentRequest
            {
                OrderId = orderId,
                Amount = order.FinalAmount,
                TransferContent = $"TH{orderId:D6}"
            });

            return View(new QrPaymentViewModel
            {
                OrderId = orderId,
                OrderCode = qr.OrderCode,
                QrImageUrl = qr.QrImageUrl,
                BankId = qr.BankId,
                AccountNumber = qr.AccountNumber,
                AccountName = qr.AccountName,
                Amount = qr.Amount,
                TransferContent = qr.TransferContent,
                IsGuest = isGuest
            });
        }

        [HttpGet]
        public IActionResult VnPayReturn()
        {
            var result = _vnPayService.ReadReturn(Request.Query);
            if (!result.IsValidSignature || !result.OrderId.HasValue)
            {
                TempData["PaymentMessage"] = result.Message;
                return RedirectToAction("Index", "Cart");
            }

            _orderService.MarkPaymentResult(
                result.OrderId.Value,
                result.IsSuccess ? 1 : 2,
                result.TransactionNo,
                result.BankCode,
                result.ResponseCode,
                result.IsSuccess ? DateTime.Now : null);

            TempData["PaymentMessage"] = result.Message;
            return RedirectToAction(nameof(Success), new { orderId = result.OrderId.Value });
        }

        [HttpGet]
        public IActionResult MoMoReturn()
        {
            var result = _moMoService.ReadReturn(Request.Query);
            return HandleMoMoCallback(result);
        }

        [HttpPost]
        public IActionResult MoMoIpn()
        {
            var result = _moMoService.ReadIpn(Request.Query);
            HandleMoMoCallback(result, silent: true);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
        public IActionResult QuickRegister([FromBody] QuickRegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Phone) || string.IsNullOrWhiteSpace(req.Password))
                return Json(new { success = false, message = "Số điện thoại và mật khẩu là bắt buộc." });

            var order = _orderService.GetOrder(req.OrderId);
            if (order == null)
                return Json(new { success = false, message = "Đơn hàng không tồn tại." });

            if (!string.Equals(order.ShippingPhone?.Trim(), req.Phone.Trim(), StringComparison.Ordinal))
                return Json(new { success = false, message = "Số điện thoại không khớp với đơn hàng." });

            var customer = req.ToCustomer(order);
            if (!_accountService.Register(customer))
                return Json(new { success = false, message = "Số điện thoại đã tồn tại hoặc đăng ký thất bại." });

            _guestOrderService.MergeToCustomer(req.Phone, customer.Id);
            return Json(new { success = true, message = "Tài khoản đã được tạo thành công." });
        }

        private IActionResult HandleMoMoCallback(MoMoIpnResult result, bool silent = false)
        {
            if (!result.IsValidSignature || !result.OrderId.HasValue)
            {
                if (!silent)
                {
                    TempData["PaymentMessage"] = result.Message;
                    return RedirectToAction("Index", "Cart");
                }
                return Ok();
            }

            _orderService.MarkPaymentResult(
                result.OrderId.Value,
                result.IsSuccess ? 1 : 2,
                result.TransId.ToString(),
                "MOMO",
                result.IsSuccess ? "0" : "1",
                result.IsSuccess ? DateTime.Now : null);

            if (silent)
                return Ok();

            TempData["PaymentMessage"] = result.Message;
            return RedirectToAction(nameof(Success), new { orderId = result.OrderId.Value });
        }

        private bool IsPaymentMethodSupported(int method, out string error)
        {
            error = "";
            switch (method)
            {
                case 1:
                    return true;
                case 2 when _vnPayService.IsConfigured:
                    return true;
                case 2:
                    error = "VNPay chưa được cấu hình. Vui lòng chọn phương thức khác.";
                    return false;
                case 3 when _moMoService.IsConfigured:
                    return true;
                case 3:
                    error = "MoMo chưa được cấu hình. Vui lòng chọn phương thức khác.";
                    return false;
                case 4 when _vietQrService.IsConfigured:
                    return true;
                case 4:
                    error = "Chuyển khoản QR chưa được cấu hình tài khoản ngân hàng.";
                    return false;
                default:
                    error = "Phương thức thanh toán không hợp lệ.";
                    return false;
            }
        }

        private void SetPaymentConfigFlags()
        {
            ViewBag.VnPayConfigured = _vnPayService.IsConfigured;
            ViewBag.MoMoConfigured = _moMoService.IsConfigured;
            ViewBag.VietQrConfigured = _vietQrService.IsConfigured;
        }

        private CheckoutViewModel BuildViewModel(List<CartItem> cart, CheckoutInput input)
        {
            var model = new CheckoutViewModel
            {
                CartItems = cart.Select(i => new CartItemViewModel
                {
                    ProductID = i.ProductID,
                    ProductName = i.ProductName,
                    Photo = i.Photo,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList(),
                Input = input
            };

            if (!string.IsNullOrWhiteSpace(input.VoucherCode) && model.TotalAmount > 0)
            {
                var voucher = _voucherService.Apply(input.VoucherCode.Trim(), model.TotalAmount);
                if (voucher.Success)
                {
                    model.DiscountAmount = voucher.DiscountAmount;
                    input.VoucherCode = input.VoucherCode.Trim().ToUpperInvariant();
                }
            }

            return model;
        }

        private CheckoutInput BuildDefaultInput()
        {
            var customerId = GetCurrentCustomerId();
            var customer = customerId > 0 ? _accountService.GetCustomerById(customerId) : null;

            return new CheckoutInput
            {
                ShippingName = customer?.CustomerName ?? "",
                ShippingPhone = customer?.Phone ?? "",
                ShippingAddress = (!string.IsNullOrWhiteSpace(customer?.Address))
                    ? customer.Address
                    : GetRememberedShippingAddress(customerId)
            };
        }

        private int GetCurrentCustomerId()
        {
            var claim = User.FindFirst("CustomerId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out var id) ? id : 0;
        }

        private string GetRememberedShippingAddress(int customerId)
            => HttpContext.Session.GetString(GetLastShippingAddressSessionKey(customerId)) ?? "";

        private void RememberShippingAddress(string address, int customerId)
        {
            if (!string.IsNullOrWhiteSpace(address))
                HttpContext.Session.SetString(GetLastShippingAddressSessionKey(customerId), address.Trim());
        }

        private static string GetLastShippingAddressSessionKey(int customerId)
            => $"{LastShippingAddressSessionKeyPrefix}{(customerId > 0 ? customerId.ToString() : "GUEST")}";

        private string GetClientIpAddress()
        {
            var forwarded = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwarded))
                return forwarded.Split(',')[0].Trim();

            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "127.0.0.1";
        }
    }
}
