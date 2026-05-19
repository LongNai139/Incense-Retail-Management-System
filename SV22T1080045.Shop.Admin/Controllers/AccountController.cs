using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.Models;
using System.Security.Claims;

namespace SV22T1080045.Shop.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IOtpService _otpService;
        private const int MaxLoginFailCount = 5;
        private const int LockMinutes = 5;

        public AccountController(IAccountService accountService, IOtpService otpService)
        {
            _accountService = accountService;
            _otpService = otpService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Vui lòng nhập đúng định dạng thông tin đăng nhập.";
                return View();
            }

            var lockUntil = HttpContext.Session.GetString($"login_lock_{model.Phone}");
            if (!string.IsNullOrWhiteSpace(lockUntil) &&
                DateTime.TryParse(lockUntil, out var lockTime) &&
                lockTime > DateTime.Now)
            {
                ViewBag.ErrorMessage = $"Bạn đã nhập sai quá nhiều lần. Vui lòng thử lại sau {LockMinutes} phút.";
                return View();
            }

            var customer = _accountService.Login(model.Phone, model.Password);

            if (customer != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, customer.CustomerName),
                    new Claim(ClaimTypes.MobilePhone, customer.Phone),
                    new Claim("CustomerId", customer.Id.ToString()),
                    new Claim(ClaimTypes.Role, customer.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.Remove($"login_fail_{model.Phone}");
                HttpContext.Session.Remove($"login_lock_{model.Phone}");
                return RedirectToAction("Index", "Home");
            }

            var failKey = $"login_fail_{model.Phone}";
            var failCount = int.TryParse(HttpContext.Session.GetString(failKey), out var c) ? c + 1 : 1;
            HttpContext.Session.SetString(failKey, failCount.ToString());

            if (failCount >= MaxLoginFailCount)
            {
                HttpContext.Session.SetString($"login_lock_{model.Phone}", DateTime.Now.AddMinutes(LockMinutes).ToString("o"));
                HttpContext.Session.Remove(failKey);
            }

            ViewBag.ErrorMessage = "Số điện thoại hoặc mật khẩu không đúng!";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Dữ liệu đăng ký không hợp lệ.";
                return View("Login");
            }

            var otpVerified = _otpService.Verify(model.Phone, OtpPurpose.Register, model.OtpCode);
            if (!otpVerified)
            {
                ViewBag.ErrorMessage = "OTP không đúng hoặc đã hết hạn.";
                return View("Login");
            }

            bool isRegistered = _accountService.Register(model.ToCustomer());
            if (isRegistered)
                return await Login(new LoginViewModel { Phone = model.Phone, Password = model.Password });

            ViewBag.ErrorMessage = "Số điện thoại đã tồn tại hoặc đăng ký thất bại.";
            return View("Login");
        }

        [HttpPost]
        public IActionResult SendRegisterOtp([FromBody] SendOtpViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Số điện thoại không hợp lệ." });

            var sessionKey = $"otp_sent_register_{model.Phone}";
            var lastSent = HttpContext.Session.GetString(sessionKey);
            if (lastSent != null && DateTime.TryParse(lastSent, out var lastSentTime) &&
                (DateTime.Now - lastSentTime).TotalSeconds < 60)
            {
                return Json(new { success = false, message = "Vui lòng chờ 60 giây trước khi gửi lại OTP." });
            }

            bool ok = _otpService.Send(model.Phone, OtpPurpose.Register);
            if (ok)
                HttpContext.Session.SetString(sessionKey, DateTime.Now.ToString("o"));

            return Json(new
            {
                success = ok,
                message = ok ? $"Đã gửi OTP đến {model.Phone}." : "Gửi OTP thất bại, vui lòng thử lại."
            });
        }

        [HttpPost]
        public IActionResult VerifyRegisterOtp([FromBody] VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu OTP không hợp lệ." });

            bool ok = _otpService.Verify(model.Phone, OtpPurpose.Register, model.Code);
            if (!ok)
                return Json(new { success = false, message = "Mã OTP không đúng hoặc đã hết hạn." });

            return Json(new { success = true, message = "OTP hợp lệ." });
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    
    }
}