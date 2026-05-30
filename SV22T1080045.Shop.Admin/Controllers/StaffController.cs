using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.Abstractions.Models.Staff;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.Models.ViewModels.Staff;
using System.Security.Claims;

namespace SV22T1080045.Shop.Controllers
{
    [Authorize(Roles = "Staff,Admin")]
    public class StaffController : Controller
    {
        private const int ProductPageSize = 12;
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        public IActionResult Index(
            string orderSearchValue = "",
            int? orderStatus = null,
            string productSearchValue = "",
            int productPage = 1)
        {
            return View(BuildDashboardModel(orderSearchValue, orderStatus, productSearchValue, productPage));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderStatus(int orderId, int status)
        {
            var result = _staffService.UpdateOrderStatus(orderId, status);
            TempData["StaffMessage"] = result.Message;
            return RedirectToAction(nameof(Index), null, null, "orders");
        }

        private StaffDashboardViewModel BuildDashboardModel(
            string orderSearchValue,
            int? orderStatus,
            string productSearchValue,
            int productPage)
        {
            var data = _staffService.GetDashboardData();
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var orders = ToOrderRows(data.Orders);
            var todayOrders = orders
                .Where(o => o.OrderDate >= today && o.OrderDate < tomorrow)
                .ToList();
            var filteredOrders = todayOrders.AsEnumerable();
            var orderKeyword = Normalize(orderSearchValue);

            if (!string.IsNullOrWhiteSpace(orderKeyword))
            {
                filteredOrders = filteredOrders.Where(o =>
                    o.OrderCode.Contains(orderKeyword, StringComparison.OrdinalIgnoreCase) ||
                    o.CustomerName.Contains(orderKeyword, StringComparison.OrdinalIgnoreCase) ||
                    o.CustomerPhone.Contains(orderKeyword, StringComparison.OrdinalIgnoreCase));
            }

            if (orderStatus.HasValue)
                filteredOrders = filteredOrders.Where(o => o.Status == orderStatus.Value);

            var allProducts = ToProductRows(data.Products);
            var productKeyword = Normalize(productSearchValue);
            var products = string.IsNullOrWhiteSpace(productKeyword)
                ? allProducts
                : allProducts.Where(p =>
                    p.ProductName.Contains(productKeyword, StringComparison.OrdinalIgnoreCase) ||
                    p.CategoryName.Contains(productKeyword, StringComparison.OrdinalIgnoreCase) ||
                    (p.Origin?.Contains(productKeyword, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();

            var orderedProducts = products
                .OrderBy(p => p.Quantity <= 0 ? 0 : p.Quantity < 5 ? 1 : 2)
                .ThenBy(p => p.Quantity)
                .ThenBy(p => p.ProductName)
                .ToList();
            var productTotalItems = orderedProducts.Count;
            var productTotalPages = Math.Max(1, (int)Math.Ceiling(productTotalItems / (double)ProductPageSize));
            var currentProductPage = Math.Clamp(productPage, 1, productTotalPages);
            var pagedProducts = orderedProducts
                .Skip((currentProductPage - 1) * ProductPageSize)
                .Take(ProductPageSize)
                .ToList();

            var pendingTooLong = todayOrders
                .Where(o => o.Status == 1 && o.OrderDate <= DateTime.Now.AddHours(-2))
                .OrderBy(o => o.OrderDate)
                .Take(5)
                .ToList();
            var failedDeliveries = todayOrders
                .Where(o => o.Status == -1)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToList();
            var lowStock = allProducts
                .Where(p => p.Quantity > 0 && p.Quantity < 5)
                .OrderBy(p => p.Quantity)
                .Take(5)
                .ToList();

            return new StaffDashboardViewModel
            {
                NewOrdersToday = todayOrders.Count(o => o.Status == 1),
                ProcessingOrders = todayOrders.Count(o => o.Status is 1 or 2),
                OrdersToDeliver = todayOrders.Count(o => o.Status == 3),
                LowStockProducts = allProducts.Count(p => p.Quantity > 0 && p.Quantity < 5),
                OrderSearchValue = orderSearchValue?.Trim() ?? "",
                OrderStatus = orderStatus,
                WorkDate = today,
                ProductSearchValue = productSearchValue?.Trim() ?? "",
                ProductPage = currentProductPage,
                ProductPageSize = ProductPageSize,
                ProductTotalItems = productTotalItems,
                ProductTotalPages = productTotalPages,
                ProductStartItem = productTotalItems == 0 ? 0 : ((currentProductPage - 1) * ProductPageSize) + 1,
                ProductEndItem = Math.Min(currentProductPage * ProductPageSize, productTotalItems),
                RecentOrders = todayOrders.OrderByDescending(o => o.OrderDate).Take(8).ToList(),
                Orders = filteredOrders.OrderByDescending(o => o.OrderDate).Take(30).ToList(),
                Products = pagedProducts,
                Customers = ToCustomerRows(data.Customers),
                Alerts = BuildAlerts(pendingTooLong, failedDeliveries, lowStock),
                Notifications = BuildNotifications(todayOrders, lowStock),
                Profile = BuildProfile()
            };
        }

        private static List<StaffOrderRowViewModel> ToOrderRows(IEnumerable<StaffOrderData> rows)
        {
            return rows.Select(row => new StaffOrderRowViewModel
            {
                Id = row.Id,
                OrderDate = row.OrderDate,
                TotalAmount = row.TotalAmount,
                Status = row.Status,
                CustomerName = row.CustomerName,
                CustomerPhone = row.CustomerPhone,
                ShippingAddress = row.ShippingAddress,
                IsGuest = row.IsGuest,
                ItemCount = row.ItemCount,
                Items = row.Items.Select(item => new StaffOrderItemViewModel
                {
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            }).ToList();
        }

        private static List<StaffProductRowViewModel> ToProductRows(IEnumerable<StaffProductData> rows)
        {
            return rows.Select(row => new StaffProductRowViewModel
            {
                Id = row.Id,
                ProductName = row.ProductName,
                CategoryName = row.CategoryName,
                UnitName = row.UnitName,
                Quantity = row.Quantity,
                SoldCount = row.SoldCount,
                PriceAfterDiscount = row.PriceAfterDiscount,
                Origin = row.Origin
            }).ToList();
        }

        private static List<StaffCustomerRowViewModel> ToCustomerRows(IEnumerable<StaffCustomerData> rows)
        {
            return rows.Select(row => new StaffCustomerRowViewModel
            {
                Id = row.Id,
                CustomerName = row.CustomerName,
                MaskedPhone = MaskPhone(row.Phone),
                MaskedEmail = MaskEmail(row.Email),
                OrderCount = row.OrderCount,
                LastOrderDate = row.LastOrderDate,
                RecentProducts = row.RecentProducts
            }).ToList();
        }

        private StaffProfileViewModel BuildProfile()
        {
            var name = User.FindFirstValue(ClaimTypes.Name) ?? "Nhan vien";
            var phone = User.FindFirstValue(ClaimTypes.MobilePhone) ?? "";
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "Staff";
            return new StaffProfileViewModel
            {
                DisplayName = name,
                Phone = phone,
                Role = role,
                AvatarInitials = BuildInitials(name)
            };
        }

        private static List<StaffAlertViewModel> BuildAlerts(
            List<StaffOrderRowViewModel> pendingTooLong,
            List<StaffOrderRowViewModel> failedDeliveries,
            List<StaffProductRowViewModel> lowStock)
        {
            var alerts = new List<StaffAlertViewModel>();

            alerts.AddRange(pendingTooLong.Select(o => new StaffAlertViewModel
            {
                Title = $"{o.OrderCode} cho xu ly qua 2 gio",
                Detail = $"{o.CustomerName} - {o.OrderDate:dd/MM HH:mm}",
                Level = "danger",
                Icon = "fa-clock"
            }));

            alerts.AddRange(failedDeliveries.Select(o => new StaffAlertViewModel
            {
                Title = $"{o.OrderCode} giao that bai",
                Detail = $"{o.CustomerName} - can lien he lai",
                Level = "warning",
                Icon = "fa-triangle-exclamation"
            }));

            alerts.AddRange(lowStock.Select(p => new StaffAlertViewModel
            {
                Title = $"{p.ProductName} sap het hang",
                Detail = $"Con {p.Quantity} {p.UnitName}",
                Level = "stock",
                Icon = "fa-box-open"
            }));

            if (alerts.Count == 0)
            {
                alerts.Add(new StaffAlertViewModel
                {
                    Title = "Khong co canh bao gap",
                    Detail = "Don hang va ton kho dang trong nguong on dinh.",
                    Level = "info",
                    Icon = "fa-circle-check"
                });
            }

            return alerts.Take(10).ToList();
        }

        private static List<StaffNotificationViewModel> BuildNotifications(
            List<StaffOrderRowViewModel> orders,
            List<StaffProductRowViewModel> lowStock)
        {
            var notifications = new List<StaffNotificationViewModel>();

            notifications.AddRange(orders
                .Where(o => o.Status == 1)
                .OrderByDescending(o => o.OrderDate)
                .Take(4)
                .Select(o => new StaffNotificationViewModel
                {
                    Message = $"Don moi {o.OrderCode}",
                    Detail = $"{o.CustomerName} can xac nhan trong ngay",
                    CreatedAt = o.OrderDate,
                    Type = "order",
                    TargetAnchor = $"order-{o.Id}"
                }));

            notifications.AddRange(lowStock.Take(3).Select(p => new StaffNotificationViewModel
            {
                Message = $"{p.ProductName} sap het hang",
                Detail = $"Con {p.Quantity} {p.UnitName}, staff chi xem de bao quan ly nhap hang",
                CreatedAt = DateTime.Now,
                Type = "stock"
            }));

            if (notifications.Count == 0)
            {
                notifications.Add(new StaffNotificationViewModel
                {
                    Message = "Khong co viec can canh bao",
                    Detail = "Thong bao duoc tao tu don moi trong ngay va san pham sap het hang.",
                    CreatedAt = DateTime.Now,
                    Type = "info"
                });
            }

            return notifications.OrderByDescending(n => n.CreatedAt).Take(8).ToList();
        }

        public static string StatusText(int status) => status switch
        {
            1 => "Cho xu ly",
            2 => "Dang chuan bi",
            3 => "Dang giao",
            4 => "Hoan thanh",
            -1 => "Giao that bai",
            _ => "Khac"
        };

        public static string StatusClass(int status) => status switch
        {
            1 => "staff-status-pending",
            2 => "staff-status-preparing",
            3 => "staff-status-delivering",
            4 => "staff-status-done",
            -1 => "staff-status-failed",
            _ => "staff-status-neutral"
        };

        private static string Normalize(string? value) => value?.Trim() ?? "";

        private static string MaskPhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return "***";

            return phone.Length <= 4 ? $"***{phone}" : $"***{phone[^4..]}";
        }

        private static string MaskEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "Chua co";

            var parts = email.Split('@');
            if (parts.Length != 2)
                return "***";

            var name = parts[0].Length <= 2 ? parts[0][0] + "***" : parts[0][..2] + "***";
            return $"{name}@{parts[1]}";
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
}
