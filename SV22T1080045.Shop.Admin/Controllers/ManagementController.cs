using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SV22T1080045.Shop.Abstractions.Models.Management;
using SV22T1080045.Shop.Abstractions.Models.Reports;
using SV22T1080045.Shop.Abstractions.Models;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Models.Mappers;
using SV22T1080045.Shop.Models.ViewModels.Management;
using SV22T1080045.Shop.Models.ViewModels.Product;
using System.Security.Claims;
using SV22T1080045.Shop.BusinessLayers;
using SV22T1080045.Shop.Admin.Extensions.Authorizations;

namespace SV22T1080045.Shop.Controllers
{
    [Authorize]
    public class ManagementController : Controller
    {
        private const int ProductPageSize = 10;

        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IUnitService _unitService;
        private readonly IRevenueReportService _revenueReportService;
        private readonly IManagementService _managementService;
        private readonly ILogger<ManagementController> _logger;

        public ManagementController(
            IProductService productService,
            ICategoryService categoryService,
            IUnitService unitService,
            IRevenueReportService revenueReportService,
            IManagementService managementService,
            ILogger<ManagementController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _unitService = unitService;
            _revenueReportService = revenueReportService;
            _managementService = managementService;
            _logger = logger;
        }

        public IActionResult Index(
            string revenuePeriod = "day",
            DateTime? revenueDate = null)
        {

            if (!User.HasAnyPermission(PermissionConstants.ViewReport, PermissionConstants.ViewRevenue, PermissionConstants.ManageSystem))
                return Forbid();

            return View("Index", BuildDashboardModel(
                revenuePeriod,
                revenueDate));
        }

        public IActionResult Products(
            int? editProductId = null,
            string searchValue = "",
            int? categoryId = null,
            string productStatus = "",
            int productPage = 1,
            int? savedProductId = null,
            string? savedAction = null)
        {
            if (!User.HasAnyPermission(PermissionConstants.ViewProduct, PermissionConstants.ManageSystem))
                return Forbid();

            return View("Products", BuildProductModel(
                editProductId,
                productForm: null,
                searchValue,
                categoryId,
                productStatus,
                productPage,
                savedProductId,
                savedAction));
        }

        [Permission(PermissionConstants.ViewOrder)]
        public IActionResult Orders(int orderPage = 1, int? selectedOrderId = null, int? status = null)
        {
            // Check if selectedOrderId is provided via query string
            if (selectedOrderId == null && int.TryParse(Request.Query["selectedOrderId"], out var queryOrderId))
            {
                selectedOrderId = queryOrderId;
            }
            return View("Orders", BuildOrderModel(orderPage, selectedOrderId, status));
        }

        [Permission(PermissionConstants.ViewCustomer)]
        public IActionResult Customers(
            int? selectedCustomerId = null,
            bool showFullCustomerHistory = false)
        {
            return View("Customers", BuildCustomerModel(selectedCustomerId, showFullCustomerHistory));
        }

        [Permission(PermissionConstants.ManageVoucher, PermissionConstants.CreateVoucher, PermissionConstants.DeleteVoucher)]
        public IActionResult Discounts(
            ManagementVoucherInput? voucherForm = null,
            string? voucherMessage = null)
        {
            return View("Discounts", BuildDiscountModel(voucherForm, voucherMessage));
        }

        [Permission(PermissionConstants.ViewReport, PermissionConstants.ViewRevenue)]
        public IActionResult Reports(
            string reportPeriod = "month",
            DateTime? reportDate = null)
        {
            return View("Reports", BuildReportModel(reportPeriod, reportDate));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveProduct(ProductEditViewModel model)
        {
            var requiredPermission = model.Id > 0 ? PermissionConstants.EditProduct : PermissionConstants.AddProduct;
            if (!User.HasAnyPermission(requiredPermission, PermissionConstants.ManageSystem))
                return Forbid();

            if (!ModelState.IsValid)
                return View("Products", BuildProductModel(model.Id > 0 ? model.Id : null, model));

            var saveAction = model.Id > 0 ? "updated" : "created";
            var savedProductId = 0;
            try
            {
                savedProductId = _productService.SaveProduct(model.ToSaveRequest());
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Products", BuildProductModel(model.Id > 0 ? model.Id : null, model));
            }

            var productPage = _productService.GetNewestPageForProduct(savedProductId, ProductPageSize);
            var productsUrl = Url.Action(nameof(Products), new { savedProductId, savedAction = saveAction, productPage }) ?? Url.Action(nameof(Products)) ?? "/";
            return Redirect(productsUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(PermissionConstants.DeleteProduct)]
        public IActionResult DeleteProduct(int productId)
        {
            var result = _productService.DeleteProduct(productId);
            if (!result)
            {
                if (IsAjaxRequest())
                    return Json(new { success = false, message = "Không thể xóa sản phẩm." });
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm.";
                return RedirectToAction(nameof(Products));
            }

            if (IsAjaxRequest())
                return Json(new { success = true, message = "Đã xóa sản phẩm thành công." });

            TempData["SuccessMessage"] = "Đã xóa sản phẩm thành công.";
            return RedirectToAction(nameof(Products));
        }

        [HttpGet]
        [Permission(PermissionConstants.ViewProduct)]
        public IActionResult GetProduct(int id)
        {
            var product = _productService.GetProduct(id);
            if (product == null)
                return Json(new { success = false, message = "Không tìm thấy sản phẩm." });

            return Json(new
            {
                success = true,
                product = new
                {
                    id = product.Id,
                    productName = product.ProductName,
                    categoryId = product.CategoryId,
                    unitId = product.UnitId,
                    importPrice = product.ImportPrice,
                    salePrice = product.SalePrice,
                    discountPercent = product.DiscountPercent,
                    priceAfterDiscount = product.DisplayPrice,
                    quantity = product.DisplayQuantity,
                    lowStockThreshold = product.DisplayLowStockThreshold,
                    soldCount = product.SoldCount,
                    rating = product.Rating,
                    reviewCount = product.ReviewCount,
                    origin = product.Origin,
                    ageYear = product.AgeYear,
                    oilContent = product.OilContent,
                    burningTime = product.BurningTime,
                    length = product.Length,
                    weight = product.Weight,
                    usageTags = product.UsageTags,
                    ingredient = product.Ingredient,
                    description = product.Description,
                    imageUrl = product.ImageUrl,
                    imageUrl2 = product.ImageUrl2,
                    imageUrl3 = product.ImageUrl3,
                    imageUrl4 = product.ImageUrl4
                }
            });
        }

        [Permission(PermissionConstants.ViewProduct)]
        public IActionResult ProductDetails(int id)
        {
            var product = _productService.GetProduct(id);
            if (product == null)
                return RedirectToAction(nameof(Products));

            var categories = _categoryService.ListCategories();
            var units = _unitService.ListUnits();

            return View("ProductDetails", new ProductManagementDetailViewModel
            {
                Product = product.ToEditViewModel(),
                Categories = ToCategoryOptions(categories),
                Units = ToUnitOptions(units)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(PermissionConstants.CreateVoucher, PermissionConstants.ManageVoucher)]
        public IActionResult SaveVoucher(ManagementVoucherInput model)
        {
            if (!ModelState.IsValid)
                return View("Discounts", BuildDiscountModel(voucherForm: model, voucherMessage: "Khong the tao ma giam gia. Vui long kiem tra lai thong tin."));

            var result = _managementService.CreateVoucher(new Voucher
            {
                Id = model.Id,
                Code = model.Code,
                Description = model.Description,
                DiscountType = (VoucherType)model.DiscountType,
                DiscountValue = model.DiscountValue,
                MaxDiscount = model.MaxDiscount,
                MinOrderAmount = model.MinOrderAmount,
                ExpiresAt = model.ExpiresAt,
                MaxUsage = model.MaxUsage,
                IsActive = model.IsActive
            });

            if (!result.Success)
                return View("Discounts", BuildDiscountModel(voucherForm: model, voucherMessage: result.Message));

            TempData["VoucherMessage"] = result.Message;
            return RedirectToAction(nameof(Discounts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(PermissionConstants.ManageVoucher)]
        public IActionResult ToggleVoucher(int id)
        {
            var result = _managementService.ToggleVoucher(id);
            if (!result.Success || result.Voucher == null)
            {
                if (IsAjaxRequest())
                    return NotFound(new { message = result.Message });

                TempData["VoucherMessage"] = result.Message;
                return RedirectToAction(nameof(Discounts));
            }

            if (IsAjaxRequest())
            {
                return Json(new
                {
                    result.Voucher.Id,
                    result.Voucher.Code,
                    result.Voucher.IsActive,
                    message = result.Message
                });
            }

            TempData["VoucherMessage"] = result.Message;
            return RedirectToAction(nameof(Discounts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(PermissionConstants.DeleteVoucher, PermissionConstants.ManageVoucher)]
        public IActionResult DeleteVoucher(int id)
        {
            var result = _managementService.DeleteVoucher(id);
            if (!result.Success)
            {
                if (IsAjaxRequest())
                    return Json(new { success = false, message = result.Message });

                TempData["VoucherMessage"] = result.Message;
                return RedirectToAction(nameof(Discounts));
            }

            if (IsAjaxRequest())
                return Json(new { success = true, message = result.Message });

            TempData["VoucherMessage"] = result.Message;
            return RedirectToAction(nameof(Discounts));
        }

        [HttpGet]
        [Permission(PermissionConstants.ExportReport)]
        public IActionResult ExportRevenueReport(string reportPeriod = "month", DateTime? reportDate = null)
        {
            var bytes = _revenueReportService.ExportRevenueReportCsv(reportPeriod, reportDate);
            var fileName = _revenueReportService.BuildRevenueReportFileName(reportPeriod, reportDate);
            return File(bytes, "text/csv; charset=utf-8", fileName);
        }

        [HttpGet]
        [Permission(PermissionConstants.ViewCustomer)]
        public IActionResult CustomerHistory(int selectedCustomerId, bool showFullCustomerHistory = false)
        {
            var customer = ToCustomerRows(_managementService.ListCustomers()).FirstOrDefault(c => c.Id == selectedCustomerId);
            if (customer == null)
                return NotFound();

            var history = ToCustomerHistory(_managementService.ListCustomerHistory(customer.Id, showFullCustomerHistory ? 20 : 3));

            return Json(new
            {
                customer = new
                {
                    customer.Id,
                    customer.CustomerName,
                    customer.OrderCount
                },
                canShowMore = !showFullCustomerHistory && customer.OrderCount > 3,
                history = history.Select(item => new
                {
                    item.OrderId,
                    item.OrderCode,
                    orderDate = item.OrderDate.ToString("dd/MM/yyyy"),
                    item.TotalAmount,
                    statusText = StatusText(item.Status),
                    item.ItemCount,
                    item.ProductNames
                })
            });
        }

        [HttpGet]
        [Permission(PermissionConstants.ViewOrderDetails, PermissionConstants.ViewOrder)]
        public IActionResult OrderDetails(int orderId)
        {
            var data = _managementService.GetOrderDetails(orderId);
            if (data == null)
                return NotFound();

            return Json(new
            {
                order = new
                {
                    data.Order.Id,
                    OrderCode = $"#TH{data.Order.Id:D6}",
                    OrderDate = data.Order.OrderDate.ToString("dd/MM/yyyy HH:mm"),
                    data.Order.TotalAmount,
                    StatusText = StatusText(data.Order.Status),
                    StatusClass = StatusClass(data.Order.Status),
                    data.Order.CustomerName,
                    data.Order.CustomerPhone,
                    data.Order.CustomerEmail,
                    data.Order.ShippingAddress,
                    data.Order.IsGuest
                },
                details = data.Details
            });
        }

        private ManagementDashboardViewModel BuildDashboardModel(
            string revenuePeriod = "day",
            DateTime? revenueDate = null)
        {
            var dashboardData = _productService.GetManagementDashboardData();
            var categories = _categoryService.ListCategories();

            var selectedRevenueDate = (revenueDate ?? DateTime.Today).Date;
            var revenueData = _revenueReportService.GetRevenueReport(revenuePeriod, selectedRevenueDate);
            var recentOrders = ToManagementOrders(_managementService.ListRecentOrders(10));

            return new ManagementDashboardViewModel
            {
                Categories = categories.Select(c => new ManagementCategoryOptionViewModel { Id = c.Id, CategoryName = c.CategoryName }).ToList(),
                BestSellerProducts = ToManagementProducts(dashboardData.BestSellers),
                LowStockProducts = ToManagementProducts(dashboardData.LowStockProducts),
                OutOfStockCount = dashboardData.OutOfStockCount,
                CategorySales = dashboardData.CategorySales.Select((item, index) => new CategorySalesSummaryViewModel
                {
                    CategoryId = item.CategoryId,
                    CategoryName = categories.FirstOrDefault(c => c.Id == item.CategoryId)?.CategoryName ?? $"Danh mục {item.CategoryId}",
                    Quantity = item.Quantity,
                    Revenue = item.Revenue,
                    RevenueShare = item.RevenueShare,
                    Color = DashboardCategoryColor(index)
                }).ToList(),
                CategorySalesTotalRevenue = dashboardData.CategoryRevenueTotal,
                BestRevenuePoint = ToRevenuePoints(revenueData.RevenuePoints).OrderByDescending(point => point.Revenue).FirstOrDefault(),
                RevenuePeriod = revenueData.Period,
                RevenueDate = selectedRevenueDate,
                RevenueRangeStart = revenueData.StartDate,
                RevenueRangeEnd = revenueData.EndDate,
                RevenueSummary = ToManagementSummary(revenueData.Summary),
                RevenuePoints = ToRevenuePoints(revenueData.RevenuePoints),
                RecentOrders = recentOrders,
                OrderStatusGroups = BuildOrderStatusGroups(recentOrders)
            };
        }

        private ProductManagementViewModel BuildProductModel(
            int? editProductId = null,
            ProductEditViewModel? productForm = null,
            string searchValue = "",
            int? categoryId = null,
            string productStatus = "",
            int productPage = 1,
            int? savedProductId = null,
            string? savedAction = null)
        {
            var categories = _categoryService.ListCategories();
            var units = _unitService.ListUnits();
            var productQuery = new ProductQuery
            {
                SearchValue = searchValue,
                SearchManagementFields = true,
                CategoryId = categoryId is > 0 ? categoryId : null,
                StockStatus = productStatus,
                SortBy = "newest",
                Page = Math.Max(1, productPage),
                PageSize = ProductPageSize
            };
            var productResult = _productService.ListProductsFiltered(productQuery);
            var totalPages = Math.Max(1, (int)Math.Ceiling(productResult.TotalCount / (double)ProductPageSize));
            var currentPage = productQuery.Page;

            productForm ??= BuildProductForm(editProductId, categories.FirstOrDefault()?.Id, units.FirstOrDefault()?.Id);

            return new ProductManagementViewModel
            {
                Categories = ToCategoryOptions(categories),
                Units = ToUnitOptions(units),
                Products = ToManagementProducts(productResult.Products),
                ProductForm = productForm,
                SearchValue = searchValue?.Trim() ?? "",
                CategoryId = categoryId,
                ProductStatus = productStatus?.Trim() ?? "",
                ProductPage = currentPage,
                ProductPageSize = ProductPageSize,
                ProductTotalCount = productResult.TotalCount,
                ProductTotalPages = totalPages,
                LastSavedProductId = savedProductId,
                LastSavedAction = savedAction
            };
        }

        private OrderManagementViewModel BuildOrderModel(int orderPage = 1, int? selectedOrderId = null, int? status = null)
        {
            const int OrderPageSize = 10;
            var currentPage = Math.Max(1, orderPage);
            var (orders, totalCount) = _managementService.ListOrdersPaginated(currentPage, OrderPageSize, status);
            var recentOrders = ToManagementOrders(orders);
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)OrderPageSize));

            return new OrderManagementViewModel
            {
                RecentOrders = recentOrders,
                OrderStatusGroups = BuildOrderStatusGroups(recentOrders),
                OrderPage = currentPage,
                OrderPageSize = OrderPageSize,
                OrderTotalCount = totalCount,
                OrderTotalPages = totalPages,
                SelectedOrderId = selectedOrderId,
                CurrentStatus = status
            };
        }

        private CustomerManagementViewModel BuildCustomerModel(
            int? selectedCustomerId = null,
            bool showFullCustomerHistory = false)
        {
            var customers = ToCustomerRows(_managementService.ListCustomers());
            var selectedCustomer = selectedCustomerId.HasValue
                ? customers.FirstOrDefault(c => c.Id == selectedCustomerId.Value)
                : customers.FirstOrDefault();

            return new CustomerManagementViewModel
            {
                Customers = customers,
                SelectedCustomer = selectedCustomer,
                SelectedCustomerHistory = selectedCustomer == null
                    ? new List<CustomerPurchaseHistoryViewModel>()
                    : ToCustomerHistory(_managementService.ListCustomerHistory(selectedCustomer.Id, showFullCustomerHistory ? 20 : 3)),
                ShowFullCustomerHistory = showFullCustomerHistory
            };
        }

        private DiscountManagementViewModel BuildDiscountModel(
            ManagementVoucherInput? voucherForm = null,
            string? voucherMessage = null)
        {
            return new DiscountManagementViewModel
            {
                VoucherForm = voucherForm ?? new ManagementVoucherInput { ExpiresAt = DateTime.Today.AddMonths(1) },
                Vouchers = ToManagementVouchers(_managementService.ListVouchers(10)),
                VoucherMessage = voucherMessage ?? TempData["VoucherMessage"] as string
            };
        }

        private ReportManagementViewModel BuildReportModel(
            string reportPeriod = "month",
            DateTime? reportDate = null)
        {
            var selectedReportDate = (reportDate ?? DateTime.Today).Date;
            var reportData = _revenueReportService.GetRevenueReport(reportPeriod, selectedReportDate);
            var categories = _categoryService.ListCategories();
            var statusCounts = new[] { 1, 2, 3, 4, -1 }
                .Select(status => new { Status = status, Count = reportData.OrderRows.Count(order => order.Status == status) })
                .ToList();
            var totalOrderCount = statusCounts.Sum(item => item.Count);

            return new ReportManagementViewModel
            {
                ReportPeriod = reportPeriod,
                ReportDate = selectedReportDate,
                ReportRangeStart = reportData.StartDate,
                ReportRangeEnd = reportData.EndDate,
                ReportSummary = ToManagementSummary(reportData.Summary),
                ReportRevenuePoints = ToRevenuePoints(reportData.RevenuePoints),
                TotalOrderCount = totalOrderCount,
                OrderStatusBreakdown = statusCounts
                    .Select(item => new ReportOrderStatusViewModel
                    {
                        Status = item.Status,
                        Label = StatusText(item.Status),
                        Count = item.Count,
                        SharePercent = totalOrderCount == 0 ? 0 : (decimal)item.Count / totalOrderCount * 100,
                        Color = ReportStatusColor(item.Status)
                    })
                    .ToList(),
                ProductSalesReports = ToProductSalesReports(reportData.ProductRows),
                TopCustomers = ToCustomerRows(reportData.CustomerRows.Take(5)),
                Categories = categories.Select(c => new ManagementCategoryOptionViewModel { Id = c.Id, CategoryName = c.CategoryName }).ToList()
            };
        }

        private static string ReportStatusColor(int status) => status switch
        {
            1 => "#d8bd77", 2 => "#86a6a0", 3 => "#c89461", 4 => "#8ca875", -1 => "#a75f4d", _ => "#aa997b"
        };

        private static List<OrderStatusGroupViewModel> BuildOrderStatusGroups(List<ManagementOrderViewModel> orders)
        {
            return new[] { 1, 2, 3, 4, -1 }
                .Select(status => new OrderStatusGroupViewModel
                {
                    Status = status,
                    StatusText = StatusText(status),
                    StatusClass = StatusClass(status),
                    Orders = orders.Where(o => o.Status == status).Take(4).ToList()
                })
                .ToList();
        }

        private static List<ManagementCategoryOptionViewModel> ToCategoryOptions(IEnumerable<Category> categories)
        {
            return categories.Select(category => new ManagementCategoryOptionViewModel
            {
                Id = category.Id,
                CategoryName = category.CategoryName
            }).ToList();
        }

        private static List<ManagementUnitOptionViewModel> ToUnitOptions(IEnumerable<Unit> units)
        {
            return units.Select(unit => new ManagementUnitOptionViewModel
            {
                Id = unit.Id,
                UnitName = unit.UnitName
            }).ToList();
        }

        private static List<ManagementProductRowViewModel> ToManagementProducts(IEnumerable<SV22T1080045.Shop.DomainModels.Product> products)
        {
            return products.Select(product => new ManagementProductRowViewModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                UnitId = product.UnitId,
                CategoryId = product.CategoryId,
                ImportPrice = product.ImportPrice,
                SalePrice = product.DisplaySalePrice,
                DiscountPercent = product.DiscountPercent,
                PriceAfterDiscount = product.DisplayPrice,
                Origin = product.Origin,
                OilContent = product.OilContent,
                Quantity = product.DisplayQuantity,
                LowStockThreshold = product.DisplayLowStockThreshold,
                SoldCount = product.SoldCount,
                ImageUrl = product.ImageUrl
            }).ToList();
        }

        private static List<ManagementVoucherViewModel> ToManagementVouchers(IEnumerable<Voucher> vouchers)
        {
            return vouchers.Select(voucher => new ManagementVoucherViewModel
            {
                Id = voucher.Id,
                Code = voucher.Code,
                DiscountType = (ManagementVoucherDiscountType)voucher.DiscountType,
                DiscountValue = voucher.DiscountValue,
                MinOrderAmount = voucher.MinOrderAmount,
                MaxUsage = voucher.MaxUsage,
                UsedCount = voucher.UsedCount,
                IsActive = voucher.IsActive
            }).ToList();
        }

        private static ManagementMetricSummary ToManagementSummary(RevenueReportSummary summary)
        {
            return new ManagementMetricSummary
            {
                Revenue = summary.Revenue,
                TotalRevenue = summary.TotalRevenue,
                PreviousRevenue = summary.PreviousRevenue,
                OrderCount = summary.OrderCount,
                CompletedOrderCount = summary.CompletedOrderCount,
                GuestOrderCount = summary.GuestOrderCount,
                NewCustomerCount = summary.NewCustomerCount
            };
        }

        private static List<RevenuePointViewModel> ToRevenuePoints(IEnumerable<RevenueReportPoint> points)
        {
            return points.Select(point => new RevenuePointViewModel
            {
                Label = point.Label,
                Revenue = point.Revenue,
                OrderCount = point.OrderCount,
                PercentOfMax = point.PercentOfMax
            }).ToList();
        }

        private static List<ManagementOrderViewModel> ToManagementOrders(IEnumerable<ManagementOrderData> rows)
        {
            return rows.Select(row => new ManagementOrderViewModel
            {
                Id = row.Id,
                OrderDate = row.OrderDate,
                TotalAmount = row.TotalAmount,
                Status = row.Status,
                CustomerName = row.CustomerName,
                CustomerPhone = row.CustomerPhone,
                ItemCount = row.ItemCount,
                ProductTypeCount = row.ProductTypeCount,
                IsGuest = row.IsGuest
            }).ToList();
        }

        private static List<CustomerManagementRowViewModel> ToCustomerRows(IEnumerable<CustomerManagementData> rows)
        {
            return rows.Select(row => new CustomerManagementRowViewModel
            {
                Id = row.Id,
                CustomerName = row.CustomerName,
                Phone = row.Phone,
                Email = row.Email,
                Address = row.Address,
                Role = row.Role,
                CreatedTime = row.CreatedTime,
                OrderCount = row.OrderCount,
                TotalSpent = row.TotalSpent,
                LastOrderDate = row.LastOrderDate
            }).ToList();
        }

        private static List<CustomerManagementRowViewModel> ToCustomerRows(IEnumerable<CustomerRevenueReportRow> rows)
        {
            return rows.Select(row => new CustomerManagementRowViewModel
            {
                Id = row.CustomerId,
                CustomerName = row.CustomerName,
                Phone = row.Phone,
                OrderCount = row.OrderCount,
                TotalSpent = row.Revenue
            }).ToList();
        }

        private static List<CustomerPurchaseHistoryViewModel> ToCustomerHistory(IEnumerable<CustomerPurchaseHistoryData> rows)
        {
            return rows.Select(row => new CustomerPurchaseHistoryViewModel
            {
                OrderId = row.OrderId,
                OrderDate = row.OrderDate,
                TotalAmount = row.TotalAmount,
                Status = row.Status,
                ItemCount = row.ItemCount,
                ProductNames = row.ProductNames
            }).ToList();
        }

        private static List<ProductSalesReportViewModel> ToProductSalesReports(IEnumerable<ProductRevenueReportRow> rows)
        {
            return rows.Take(10).Select(row => new ProductSalesReportViewModel
            {
                ProductId = row.ProductId,
                ProductName = row.ProductName,
                Quantity = row.Quantity,
                Revenue = row.Revenue,
                OrderCount = row.OrderCount
            }).ToList();
        }

        private bool IsAjaxRequest()
        {
            return string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
        }

        private static string DashboardCategoryColor(int index) =>
            new[] { "#c9a961", "#8a6d28", "#d4b86a", "#a67c3d", "#b98b36", "#52745b" }[Math.Max(0, index) % 6];

        private ProductEditViewModel BuildProductForm(int? editProductId, int? defaultCategoryId, int? defaultUnitId)
        {
            if (editProductId.HasValue && editProductId.Value > 0)
            {
                var product = _productService.GetProduct(editProductId.Value);
                if (product != null)
                    return product.ToEditViewModel();
            }

            return new ProductEditViewModel
            {
                CategoryId = defaultCategoryId ?? 0,
                UnitId = defaultUnitId ?? 0,
                Quantity = 0,
                LowStockThreshold = 5,
                SoldCount = 0,
                Rating = 5,
                ReviewCount = 0
            };
        }

        public static string StatusText(int status) => status switch
        {
            1 => "Chờ xử lý",
            2 => "Đang chuẩn bị",
            3 => "Đang giao",
            4 => "Hoàn thành",
            -1 => "Đã hủy",
            _ => "Khác"
        };

        public static string StatusClass(int status) => status switch
        {
            1 => "status-pending",
            2 => "status-ready",
            3 => "status-shipped",
            4 => "status-done",
            -1 => "status-cancel",
            _ => "status-ready"
        };
    }
}
