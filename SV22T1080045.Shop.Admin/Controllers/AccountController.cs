//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using SV22T1080045.Shop.BusinessLayers;
//using SV22T1080045.Shop.DomainModels;
//using System.Security.Claims;

//namespace SV22T1080045.Shop.Controllers
//{
//    public class AccountController : Controller
//    {
//        private readonly AccountService _accountService;

//        public AccountController(AccountService accountService)
//        {
//            _accountService = accountService;
//        }

//        [HttpGet]
//        public IActionResult Login()
//        {
//            // Nếu đã đăng nhập rồi thì đá về trang chủ
//            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Login(string email, string password)
//        {
//            var user = _accountService.Login(email, password);
//            if (user == null)
//            {
//                ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng.";
//                return View();
//            }

//            // Tạo thông tin định danh 
//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.NameIdentifier, user.CustomerID.ToString()),
//                new Claim(ClaimTypes.Name, user.CustomerName),
//                new Claim(ClaimTypes.Email, user.Email),
//                new Claim("Phone", user.Phone ?? ""),
//                new Claim("Address", user.Address ?? "")
//            };

//            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//            var authProperties = new AuthenticationProperties { IsPersistent = true }; 

   
//            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

//            return RedirectToAction("Index", "Home");
//        }

//        public async Task<IActionResult> Logout()
//        {
//            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//            return RedirectToAction("Login");
//        }

//        [HttpGet]
//        public IActionResult Register() => View();

//        [HttpPost]
//        public IActionResult Register(Customer data, string confirmPassword)
//        {
//            if (data.Password != confirmPassword)
//            {
//                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
//                return View(data);
//            }

//            string result = _accountService.Register(data);
//            if (string.IsNullOrEmpty(result))
//            {
//                ViewBag.Success = "Đăng ký thành công! Vui lòng đăng nhập.";
//                return View("Login");
//            }

//            ViewBag.Error = result;
//            return View(data);
//        }

//        [Authorize]
//        public IActionResult Profile()
//        {
//            int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
//            var user = _accountService.GetCustomer(id);
//            return View(user);
//        }

//        [Authorize]
//        [HttpPost]
//        public IActionResult UpdateProfile(string customerName, string phone, string address)
//        {
//            int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
//            _accountService.UpdateProfile(id, customerName, phone, address);

//            // Cập nhật lại Cookie để hiển thị tên mới ngay lập tức (nếu muốn)
//            // Hoặc đơn giản là load lại trang
//            return RedirectToAction("Profile");
//        }

//        [Authorize]
//        [HttpPost]
//        public IActionResult ChangePassword(string oldPass, string newPass, string confirmPass)
//        {
//            if (newPass != confirmPass)
//            {
//                TempData["PassError"] = "Mật khẩu xác nhận không khớp.";
//                return RedirectToAction("Profile");
//            }

//            int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
//            bool result = _accountService.ChangePassword(id, oldPass, newPass);

//            if (result) TempData["PassSuccess"] = "Đổi mật khẩu thành công!";
//            else TempData["PassError"] = "Mật khẩu cũ không đúng.";

//            return RedirectToAction("Profile");
//        }

//        public IActionResult AccessDenied()
//        {
//            return View();
//        }
//    }
//}