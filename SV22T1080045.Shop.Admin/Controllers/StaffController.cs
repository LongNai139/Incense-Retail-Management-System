using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.Admin.Extensions.Authorizations;
using SV22T1080045.Shop.Abstractions.Models.Staff;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Models.ViewModels.Staff;
using System.Security.Claims;

namespace SV22T1080045.Shop.Controllers
{
    [Authorize]
    public class StaffController : Controller
    {
        private const int ProductPageSize = 12;
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [Permission(PermissionConstants.ViewOrder, PermissionConstants.ViewProduct, PermissionConstants.ViewCustomer)]
        public IActionResult Index()
        {
            return View(BuildDashboardModel("", null, "", 1));
        }

        [Permission(PermissionConstants.ViewOrder)]
        public IActionResult Orders(
            string orderSearchValue = "",
            int? orderStatus = null)
        {
            return View(BuildOrderModel(orderSearchValue, orderStatus));
        }

        [Permission(PermissionConstants.ViewProduct)]
        public IActionResult Products(
            string productSearchValue = "",
            int productPage = 1)
        {
            return View(BuildProductModel(productSearchValue, productPage));
        }

        [Permission(PermissionConstants.ViewCustomer)]
        public IActionResult Customers()
        {
            return View(BuildCustomerModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(PermissionConstants.UpdateOrderStatus)]
        public IActionResult UpdateOrderStatus(int orderId, int status)
        {
            var result = _staffService.UpdateOrderStatusWithInventoryCheck(orderId, status);
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
            var productResult = _staffService.QueryProducts(productSearchValue, productPage, ProductPageSize);
            var todayOrders = ToOrderRows(_staffService.GetTodayOrders(30, orderSearchValue, orderStatus));
            var pagedProducts = ToProductRows(productResult.Items);
            var lowStock = ToProductRows(data.LowStockProducts);
            var pendingTooLong = ToOrderRows(data.PendingTooLongOrders);
            var failedDeliveries = ToOrderRows(data.FailedDeliveryOrders);
            var currentProductPage = productResult.Page;
            var productTotalItems = productResult.TotalCount;
            var productTotalPages = Math.Max(1, (int)Math.Ceiling(productTotalItems / (double)ProductPageSize));
            var today = DateTime.Today;

            return new StaffDashboardViewModel
            {
                NewOrdersToday = data.NewOrdersToday,
                ProcessingOrders = data.ProcessingOrders,
                OrdersToDeliver = data.OrdersToDeliver,
                LowStockProducts = data.LowStockCount,
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
                RecentOrders = ToOrderRows(data.RecentOrders),
                Orders = todayOrders,
                Products = pagedProducts,
                Customers = ToCustomerRows(data.Customers),
                Alerts = BuildAlerts(pendingTooLong, failedDeliveries, lowStock),
                Notifications = BuildNotifications(ToOrderRows(data.TodayOrders), lowStock),
                Profile = BuildProfile()
            };
        }

        private const int RecentCustomerLimit = 20;

        private StaffOrderViewModel BuildOrderModel(
            string orderSearchValue,
            int? orderStatus)
        {
            var today = DateTime.Today;
            // Chỉ tải đơn hôm nay (lọc trong SQL), không tải sản phẩm/khách như trước.
            var orders = ToOrderRows(_staffService.GetTodayOrders(500, orderSearchValue, orderStatus));

            return new StaffOrderViewModel
            {
                OrderSearchValue = orderSearchValue?.Trim() ?? "",
                OrderStatus = orderStatus,
                WorkDate = today,
                Orders = orders
            };
        }

        private StaffProductViewModel BuildProductModel(
            string productSearchValue,
            int productPage)
        {
            // Tìm kiếm + sắp xếp + phân trang chạy trong SQL; chỉ lấy đúng 1 trang.
            var result = _staffService.QueryProducts(productSearchValue, productPage, ProductPageSize);
            var totalItems = result.TotalCount;
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)ProductPageSize));

            return new StaffProductViewModel
            {
                ProductSearchValue = productSearchValue?.Trim() ?? "",
                ProductPage = result.Page,
                ProductPageSize = ProductPageSize,
                ProductTotalItems = totalItems,
                ProductTotalPages = totalPages,
                ProductStartItem = totalItems == 0 ? 0 : ((result.Page - 1) * ProductPageSize) + 1,
                ProductEndItem = Math.Min(result.Page * ProductPageSize, totalItems),
                Products = ToProductRows(result.Items)
            };
        }

        private StaffCustomerViewModel BuildCustomerModel()
        {
            return new StaffCustomerViewModel
            {
                Customers = ToCustomerRows(_staffService.GetRecentCustomers(RecentCustomerLimit))
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
                StatusText = StatusText(row.Status),
                StatusClass = StatusClass(row.Status),
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
                Title = $"{o.OrderCode} chờ xử lý quá 2 giờ",
                Detail = $"{o.CustomerName} - {o.OrderDate:dd/MM HH:mm}",
                Level = "danger",
                Icon = "fa-clock"
            }));

            alerts.AddRange(failedDeliveries.Select(o => new StaffAlertViewModel
            {
                Title = $"{o.OrderCode} giao thất bại",
                Detail = $"{o.CustomerName} - cần liên hệ lại",
                Level = "warning",
                Icon = "fa-triangle-exclamation"
            }));

            alerts.AddRange(lowStock.Select(p => new StaffAlertViewModel
            {
                Title = $"{p.ProductName} sắp hết hàng",
                Detail = $"Còn {p.Quantity} {p.UnitName}",
                Level = "stock",
                Icon = "fa-box-open"
            }));

            if (alerts.Count == 0)
            {
                alerts.Add(new StaffAlertViewModel
                {
                    Title = "Không có cảnh báo gấp",
                    Detail = "Đơn hàng và tồn kho đang trong ngưỡng ổn định.",
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
                    Message = $"Đơn mới {o.OrderCode}",
                    Detail = $"{o.CustomerName} cần xác nhận trong ngày",
                    CreatedAt = o.OrderDate,
                    Type = "order",
                    TargetAnchor = $"order-{o.Id}"
                }));

            notifications.AddRange(lowStock.Take(3).Select(p => new StaffNotificationViewModel
            {
                Message = $"{p.ProductName} sắp hết hàng",
                Detail = $"Còn {p.Quantity} {p.UnitName} — báo quản lý nhập hàng",
                CreatedAt = DateTime.Now,
                Type = "stock"
            }));

            if (notifications.Count == 0)
            {
                notifications.Add(new StaffNotificationViewModel
                {
                    Message = "Không có việc cần cảnh báo",
                    Detail = "Thông báo được tạo từ đơn mới trong ngày và sản phẩm sắp hết hàng.",
                    CreatedAt = DateTime.Now,
                    Type = "info"
                });
            }

            return notifications.OrderByDescending(n => n.CreatedAt).Take(8).ToList();
        }

        public static string StatusText(int status) => status switch
        {
            1 => "Chờ xử lý",
            2 => "Đang chuẩn bị",
            3 => "Đang giao",
            4 => "Hoàn thành",
            -1 => "Hủy đơn",
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

        private static string MaskPhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return "***";

            return phone.Length <= 4 ? $"***{phone}" : $"***{phone[^4..]}";
        }

        private static string MaskEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "Chưa có";

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
