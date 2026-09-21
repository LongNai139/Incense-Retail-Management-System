using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.Common.Model.Users;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Models;
using System.Security.Claims;

namespace SV22T1080045.Shop.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;
        private const int MaxLoginFailCount = 5;
        private const int LockMinutes = 5;

        public AccountController(IAccountService accountService, IConfiguration configuration, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.IsInRole(CustomerRoles.Admin))
                return RedirectToAction("Index", "Management");

            if (User.IsInRole(CustomerRoles.Staff))
                return RedirectToAction("Index", "Staff");

            ViewBag.AuthMode = "Login";
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.StorefrontUrl = GetStorefrontUrl();
            ViewBag.InfoMessage = TempData["LoginMessage"] as string;
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult AccessDenied()
        {
            ViewBag.StorefrontUrl = GetStorefrontUrl();

            if (User.IsInRole(CustomerRoles.Staff))
            {
                ViewBag.Title = "Không có quyền truy cập";
                ViewBag.Message = "Tài khoản Staff chỉ dùng khu vực /Staff. Trang Quản lý (/Management) dành cho Admin.";
                ViewBag.WorkspaceUrl = Url.Action("Index", "Staff");
                ViewBag.WorkspaceLabel = "Về khu Staff";
                return View();
            }

            if (User.IsInRole(CustomerRoles.Admin))
                return RedirectToAction("Index", "Management");

            return RedirectToAction(nameof(Login));
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult Register()
        {
            return Redirect($"{GetStorefrontUrl()}/Account/Register");
        }

        /// <summary>
        /// Nhận đăng nhập từ cửa hàng (7126) sau khi xác thực — tạo cookie quản trị trên 7127.
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> BridgeLogin(UserSignInRequest model, string? returnUrl = null)
        {
            var customer = _accountService.Login(model.Phone, model.Password);
            if (customer == null || !IsBackofficeRole(customer.Role))
                return RedirectToAction(nameof(Login), new { returnUrl });

            await SignInCustomerAsync(customer);

            if (string.Equals(customer.Role, CustomerRoles.Admin, StringComparison.OrdinalIgnoreCase))
                return RedirectToLocal(returnUrl, () => RedirectToAction("Index", "Management"));

            return RedirectToLocal(returnUrl, () => RedirectToAction("Index", "Staff"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserSignInRequest model, string? returnUrl = null)
        {
            _logger.LogInformation("Login attempt. Phone: {Phone}, Password length: {PasswordLength}", model.Phone, model.Password?.Length);

            ViewBag.AuthMode = "Login";
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.StorefrontUrl = GetStorefrontUrl();

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                _logger.LogWarning("ModelState invalid: {Errors}", errors);
                ModelState.AddModelError("", errors);
                return View(new LoginViewModel { Phone = model.Phone, Password = model.Password });
            }

            var lockUntil = HttpContext.Session.GetString($"login_lock_{model.Phone}");
            if (!string.IsNullOrWhiteSpace(lockUntil) &&
                DateTime.TryParse(lockUntil, out var lockTime) &&
                lockTime > DateTime.Now)
            {
                _logger.LogWarning("Account locked until: {LockUntil}", lockUntil);
                ModelState.AddModelError("", $"Bạn đã nhập sai quá nhiều lần. Vui lòng thử lại sau {LockMinutes} phút.");
                return View(new LoginViewModel { Phone = model.Phone, Password = model.Password });
            }

            var customer = _accountService.Login(model.Phone, model.Password);
            _logger.LogInformation("Login result: {Result}", customer != null ? "Success" : "Failed");

            if (customer != null)
            {
                _logger.LogInformation("Customer found. ID: {CustomerId}, Role: {Role}", customer.Id, customer.Role);

                if (!IsBackofficeRole(customer.Role))
                {
                    _logger.LogWarning("Role check failed. Role: {Role}", customer.Role);
                    ModelState.AddModelError("", $"Tài khoản khách hàng chỉ đăng nhập tại cửa hàng ({GetStorefrontUrl()}).");
                    ViewBag.StorefrontUrl = GetStorefrontUrl();
                    return View(new LoginViewModel { Phone = model.Phone, Password = model.Password });
                }

                await SignInCustomerAsync(customer);

                HttpContext.Session.Remove($"login_fail_{model.Phone}");
                HttpContext.Session.Remove($"login_lock_{model.Phone}");

                _logger.LogInformation("Login successful. Customer ID: {CustomerId}, Role: {Role}", customer.Id, customer.Role);
                _logger.LogInformation("Redirecting to Admin: {IsAdmin}", string.Equals(customer.Role, CustomerRoles.Admin, StringComparison.OrdinalIgnoreCase));

                if (string.Equals(customer.Role, CustomerRoles.Admin, StringComparison.OrdinalIgnoreCase))
                    return RedirectToLocal(returnUrl, () => RedirectToAction("Index", "Management"));

                return RedirectToLocal(returnUrl, () => RedirectToAction("Index", "Staff"));
            }

            var failKey = $"login_fail_{model.Phone}";
            var failCount = int.TryParse(HttpContext.Session.GetString(failKey), out var c) ? c + 1 : 1;
            HttpContext.Session.SetString(failKey, failCount.ToString());

            _logger.LogWarning("Login failed. Fail count: {FailCount}", failCount);

            if (failCount >= MaxLoginFailCount)
            {
                _logger.LogWarning("Account locked. Phone: {Phone}", model.Phone);
                HttpContext.Session.SetString($"login_lock_{model.Phone}", DateTime.Now.AddMinutes(LockMinutes).ToString("o"));
                HttpContext.Session.Remove(failKey);
                ModelState.AddModelError("", $"Bạn đã nhập sai quá nhiều lần. Vui lòng thử lại sau {LockMinutes} phút.");
            }
            else
            {
                ModelState.AddModelError("", "Số điện thoại hoặc mật khẩu không đúng!");
            }

            return View(new LoginViewModel { Phone = model.Phone, Password = model.Password });
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

        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> LogoutBridge()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Content("OK");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            ViewBag.StorefrontLogoutUrl = $"{GetStorefrontUrl()}/Account/LogoutBridge";
            ViewBag.StorefrontLoginUrl = $"{GetStorefrontUrl()}/Account/Login";
            return View("StorefrontLogoutBridge");
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

        private string GetStorefrontUrl()
        {
            return (_configuration["AppHosts:StorefrontUrl"] ?? "https://localhost:7126").TrimEnd('/');
        }

        private static bool IsBackofficeRole(string? role)
        {
            return string.Equals(role, CustomerRoles.Staff, StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, CustomerRoles.Admin, StringComparison.OrdinalIgnoreCase);
        }

        private IActionResult RedirectToLocal(string? returnUrl, Func<IActionResult> fallback)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return fallback();
        }
    }
}
