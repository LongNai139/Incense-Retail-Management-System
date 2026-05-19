using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.Models.Requests.Auth;

namespace SV22T1080045.Shop.Controllers
{
    public class OrderLookupController : Controller
    {
        private readonly IGuestOrderService _guestOrderService;
        private readonly IOrderService _orderService;
        private readonly IOtpService _otpService;

        public OrderLookupController(IOtpService otpService, IGuestOrderService guestOrderService, IOrderService orderService)
        {
            _otpService = otpService;
            _guestOrderService = guestOrderService;
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult SendOtp([FromBody] SendOtpRequest req)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Số điện thoại không hợp lệ." });

            if (string.IsNullOrWhiteSpace(req.Phone))
                return Json(new { success = false, message = "Vui lòng nhập số điện thoại." });

            var sessionKey = $"otp_sent_{req.Phone}";
            var lastSent = HttpContext.Session.GetString(sessionKey);
            if (lastSent != null &&
                DateTime.TryParse(lastSent, out var sentAt) &&
                (DateTime.Now - sentAt).TotalSeconds < 60)
            {
                return Json(new { success = false, message = "Vui lòng chờ 60 giây trước khi gửi lại." });
            }

            var ok = _otpService.Send(req.Phone, OtpPurpose.OrderLookup);
            if (ok)
                HttpContext.Session.SetString(sessionKey, DateTime.Now.ToString("o"));

            return Json(new
            {
                success = ok,
                message = ok ? $"Đã gửi mã OTP về số {req.Phone}" : "Gửi OTP thất bại, vui lòng thử lại."
            });
        }

        [HttpPost]
        public IActionResult Verify([FromBody] VerifyOtpRequest req)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu OTP không hợp lệ." });

            var ok = _otpService.Verify(req.Phone, OtpPurpose.OrderLookup, req.Code);
            if (!ok)
                return Json(new { success = false, message = "Mã OTP không đúng hoặc đã hết hạn." });

            HttpContext.Session.SetString("verified_phone", req.Phone);
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult OrdersJson()
        {
            var phone = HttpContext.Session.GetString("verified_phone");
            if (string.IsNullOrEmpty(phone))
                return Json(new { error = "Chưa xác thực" });

            var orders = _guestOrderService.GetOrdersByPhone(phone);
            var result = orders.Select(o => new
            {
                orderId = o.Id,
                orderDate = o.OrderDate,
                status = o.Status,
                total = o.TotalAmount,
                itemCount = _orderService.GetOrderDetails(o.Id).Sum(d => d.Quantity)
            });

            return Json(result);
        }
    }
}
