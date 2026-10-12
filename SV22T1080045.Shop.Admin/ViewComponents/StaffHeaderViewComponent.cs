using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Admin.Extensions.Authorizations;

namespace SV22T1080045.Shop.Admin.ViewComponents
{
    public class StaffHeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var modules = new List<StaffMenuViewModel>();
            var user = User as ClaimsPrincipal;

            // Module Dashboard - luôn hiển thị cho Staff
            modules.Add(new StaffMenuViewModel() { Name = "Dashboard", Module = "dashboard", Url = "/Staff/Index/", Icon = "fa-gauge-high", IsActive = true });

            // Module Đơn hàng - cần permission view_order
            if (user != null && user.HasAnyPermission(PermissionConstants.ViewOrder))
            {
                modules.Add(new StaffMenuViewModel() { Name = "Đơn hàng", Module = "orders", Url = "/Staff/Orders/", Icon = "fa-receipt" });
            }

            // Module Sản phẩm - cần permission view_product
            if (user != null && user.HasAnyPermission(PermissionConstants.ViewProduct))
            {
                modules.Add(new StaffMenuViewModel() { Name = "Sản phẩm", Module = "products", Url = "/Staff/Products/", Icon = "fa-boxes-stacked" });
            }

            // Module Khách hàng - cần permission view_customer
            if (user != null && user.HasAnyPermission(PermissionConstants.ViewCustomer))
            {
                modules.Add(new StaffMenuViewModel() { Name = "Khách hàng", Module = "customers", Url = "/Staff/Customers/", Icon = "fa-users" });
            }

            var currentPath = Request.Path.Value?.Replace('/', ' ').Trim() ?? "";
            modules = modules.Select(v => new StaffMenuViewModel()
            {
                Id = v.Id,
                Icon = v.Icon,
                Name = v.Name,
                Url = v.Url,
                Module = v.Module,
                IsActive = !string.IsNullOrWhiteSpace(v.Url) && v.Url.Replace('/', ' ').Trim().Equals(currentPath, StringComparison.OrdinalIgnoreCase)
            }).ToList();

            var navigationVm = new StaffNavigationViewModel()
            {
                UserInfo = BuildUserInfo(user),
                Modules = modules
            };

            return View("_StaffHeader", navigationVm);
        }

        private StaffUserInfoViewModel BuildUserInfo(ClaimsPrincipal? user)
        {
            var name = user?.FindFirstValue(ClaimTypes.Name) ?? "Nhan vien";
            var phone = user?.FindFirstValue(ClaimTypes.MobilePhone) ?? "";
            var role = user?.FindFirstValue(ClaimTypes.Role) ?? "Staff";
            var roleDisplay = role switch
            {
                CustomerRoles.Admin => "Quản trị viên",
                CustomerRoles.Staff => "Nhân viên",
                _ => "Người dùng"
            };

            return new StaffUserInfoViewModel
            {
                DisplayName = name,
                Phone = phone,
                Role = role,
                RoleDisplay = roleDisplay,
                AvatarInitials = BuildInitials(name)
            };
        }

        private static string BuildInitials(string name)
        {
            var parts = name
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .TakeLast(2)
                .Select(p => p[0].ToString().ToUpperInvariant());

            var initials = string.Concat(parts);
            return string.IsNullOrWhiteSpace(initials) ? "ST" : initials;
        }
    }

    public class StaffNavigationViewModel
    {
        public StaffUserInfoViewModel? UserInfo { get; set; }
        public List<StaffMenuViewModel> Modules { get; set; } = new();
    }

    public class StaffUserInfoViewModel
    {
        public string DisplayName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Role { get; set; } = "";
        public string RoleDisplay { get; set; } = "";
        public string AvatarInitials { get; set; } = "";
    }

    public class StaffMenuViewModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Module { get; set; } = "";
        public string Url { get; set; } = "";
        public string Icon { get; set; } = "";
        public bool IsActive { get; set; }
    }
}
