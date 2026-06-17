using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Models;
using System.Security.Claims;

namespace SV22T1080045.Shop.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IOtpService _otpService;
        private const int MaxLoginFailCount = 5;
        private const int MaxRegisterOtpVerifyFailCount = 5;
        private const int RegisterOtpVerifiedMinutes = 5;
        private const int LockMinutes = 5;

        public AccountController(IAccountService accountService, IOtpService otpService)
        {
            _accountService = accountService;
            _otpService = otpService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.AuthMode = "Login";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewBag.AuthMode = "Login";

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
                await SignInCustomerAsync(customer);

                HttpContext.Session.Remove($"login_fail_{model.Phone}");
                HttpContext.Session.Remove($"login_lock_{model.Phone}");
                if (string.Equals(customer.Role, "Staff", StringComparison.OrdinalIgnoreCase))
                    return RedirectToAction("Index", "Staff");

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

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.AuthMode = "Register";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PrepareRegisterView(model, "Dữ liệu đăng ký không hợp lệ.");
                return View("Register");
            }

            var otpVerified = IsRegisterOtpVerified(model.Phone) ||
                _otpService.Verify(model.Phone, OtpPurpose.Register, model.OtpCode);
            if (!otpVerified)
            {
                PrepareRegisterView(model, "OTP không đúng hoặc đã hết hạn.");
                return View("Register");
            }

            bool isRegistered = _accountService.Register(model.ToCustomer());
            if (isRegistered)
            {
                HttpContext.Session.Remove(GetRegisterOtpVerifiedKey(model.Phone));
                return await Login(new LoginViewModel { Phone = model.Phone, Password = model.Password });
            }

            PrepareRegisterView(model, "Số điện thoại đã tồn tại hoặc đăng ký thất bại.");
            return View("Register");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendRegisterOtp([FromBody] SendOtpViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Số điện thoại không hợp lệ." });

            var sessionKey = $"otp_sent_register_{model.Phone}";
            var lastSent = HttpContext.Session.GetString(sessionKey);
            if (lastSent != null &&
                DateTime.TryParse(lastSent, out var lastSentTime) &&
                (DateTime.Now - lastSentTime).TotalSeconds < 60)
            {
                return Json(new { success = false, message = "Vui lòng chờ 60 giây trước khi gửi lại OTP." });
            }

            bool ok = _otpService.Send(model.Phone, OtpPurpose.Register);
            if (ok)
            {
                HttpContext.Session.SetString(sessionKey, DateTime.Now.ToString("o"));
                HttpContext.Session.Remove(GetRegisterOtpVerifiedKey(model.Phone));
            }

            return Json(new
            {
                success = ok,
                message = ok ? $"Đã gửi OTP đến {model.Phone}." : "Gửi OTP thất bại, vui lòng thử lại."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyRegisterOtp([FromBody] VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu OTP không hợp lệ." });

            var lockKey = $"otp_verify_lock_register_{model.Phone}";
            var lockedUntil = HttpContext.Session.GetString(lockKey);
            if (!string.IsNullOrWhiteSpace(lockedUntil) &&
                DateTime.TryParse(lockedUntil, out var lockTime) &&
                lockTime > DateTime.Now)
            {
                return Json(new { success = false, message = $"Bạn đã nhập sai OTP quá nhiều lần. Vui lòng thử lại sau {LockMinutes} phút." });
            }

            bool ok = _otpService.Verify(model.Phone, OtpPurpose.Register, model.Code);
            if (!ok)
            {
                var failKey = $"otp_verify_fail_register_{model.Phone}";
                var failCount = int.TryParse(HttpContext.Session.GetString(failKey), out var c) ? c + 1 : 1;
                HttpContext.Session.SetString(failKey, failCount.ToString());

                if (failCount >= MaxRegisterOtpVerifyFailCount)
                {
                    HttpContext.Session.SetString(lockKey, DateTime.Now.AddMinutes(LockMinutes).ToString("o"));
                    HttpContext.Session.Remove(failKey);
                }

                return Json(new { success = false, message = "Mã OTP không đúng hoặc đã hết hạn." });
            }

            HttpContext.Session.Remove($"otp_verify_fail_register_{model.Phone}");
            HttpContext.Session.Remove(lockKey);
            HttpContext.Session.SetString(GetRegisterOtpVerifiedKey(model.Phone), DateTime.Now.ToString("o"));

            return Json(new { success = true, message = "OTP hợp lệ." });
        }

        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            var customer = GetCurrentCustomer();
            if (customer == null)
                return RedirectToAction(nameof(Login));

            return View(customer.ToProfileViewModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId <= 0)
                return RedirectToAction(nameof(Login));

            if (!ModelState.IsValid)
                return View(model);

            var updated = _accountService.UpdateProfile(customerId, model.CustomerName, model.Email, model.Address);
            if (!updated)
            {
                ModelState.AddModelError(string.Empty, "Không thể cập nhật thông tin cá nhân. Vui lòng thử lại.");
                return View(model);
            }

            var customer = _accountService.GetCustomerById(customerId);
            if (customer != null)
                await SignInCustomerAsync(customer);

            TempData["ProfileSuccess"] = "Đã lưu thông tin cá nhân.";
            return RedirectToAction(nameof(Profile));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private void PrepareRegisterView(RegisterViewModel model, string message)
        {
            ViewBag.AuthMode = "Register";
            ViewBag.ErrorMessage = message;
            ViewBag.RegisterName = model.CustomerName;
            ViewBag.RegisterPhone = model.Phone;
        }

        private bool IsRegisterOtpVerified(string phone)
        {
            var verifiedAt = HttpContext.Session.GetString(GetRegisterOtpVerifiedKey(phone));
            return !string.IsNullOrWhiteSpace(verifiedAt) &&
                DateTime.TryParse(verifiedAt, out var verifiedTime) &&
                DateTime.Now <= verifiedTime.AddMinutes(RegisterOtpVerifiedMinutes);
        }

        private static string GetRegisterOtpVerifiedKey(string phone)
        {
            return $"otp_verified_register_{phone}";
        }

        private async Task SignInCustomerAsync(Customer customer)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(ClaimTypes.Name, customer.CustomerName),
                new Claim(ClaimTypes.MobilePhone, customer.Phone),
                new Claim("CustomerId", customer.Id.ToString()),
                new Claim(ClaimTypes.Role, customer.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        private Customer? GetCurrentCustomer()
        {
            var customerId = GetCurrentCustomerId();
            return customerId <= 0 ? null : _accountService.GetCustomerById(customerId);
        }

        private int GetCurrentCustomerId()
        {
            var customerIdClaim = User.FindFirst("CustomerId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            return customerIdClaim != null && int.TryParse(customerIdClaim.Value, out var customerId)
                ? customerId
                : 0;
        }
    }
}
