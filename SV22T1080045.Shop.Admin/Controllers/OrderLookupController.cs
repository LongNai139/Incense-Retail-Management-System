using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers;

namespace SV22T1080045.Shop.Admin.Controllers
{
    /// <summary>Tra cứu đơn hàng bằng SĐT + OTP — không cần đăng nhập</summary>
    public class OrderLookupController : Controller
    {
        private readonly IOtpService _otpService;
        private readonly IGuestOrderService _guestOrderService;

        public OrderLookupController(IOtpService otpService, IGuestOrderService guestOrderService)
        {
            _otpService = otpService;
            _guestOrderService = guestOrderService;
        }

        // GET /tra-cuu-don-hang
        [HttpGet]
        public IActionResult Index() => View();

        // POST /OrderLookup/SendOtp ── AJAX
        [HttpPost]
        public IActionResult SendOtp([FromBody] SendOtpRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Phone))
                return Json(new { success = false, message = "Vui lòng nhập số điện thoại." });

            // Rate-limit đơn giản: 1 lần / 60 giây / SĐT
            var sessionKey = $"otp_sent_{req.Phone}";
            var lastSent = HttpContext.Session.GetString(sessionKey);
            if (lastSent != null &&
                (DateTime.Now - DateTime.Parse(lastSent)).TotalSeconds < 60)
            {
                return Json(new { success = false, message = "Vui lòng chờ 60 giây trước khi gửi lại." });
            }

            bool ok = _otpService.Send(req.Phone, OtpPurpose.OrderLookup);
            if (ok) HttpContext.Session.SetString(sessionKey, DateTime.Now.ToString("o"));

            return Json(new
            {
                success = ok,
                message = ok
                    ? $"Đã gửi mã OTP về số {req.Phone}"
                    : "Gửi OTP thất bại, vui lòng thử lại."
            });
        }

        // POST /OrderLookup/Verify ── AJAX: xác thực OTP
        [HttpPost]
        public IActionResult Verify([FromBody] VerifyOtpRequest req)
        {
            bool ok = _otpService.Verify(req.Phone, OtpPurpose.OrderLookup, req.Code);
            if (!ok)
                return Json(new { success = false, message = "Mã OTP không đúng hoặc đã hết hạn." });

            // Lưu SĐT đã xác thực vào Session
            HttpContext.Session.SetString("verified_phone", req.Phone);
            return Json(new { success = true });
        }

        // GET /OrderLookup/OrdersJson ── trả JSON danh sách đơn
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
                itemCount = 0  // TODO: join với OrderDetails nếu cần
            });

            return Json(result);
        }
    }

    public record SendOtpRequest(string Phone);
    public record VerifyOtpRequest(string Phone, string Code);
}
