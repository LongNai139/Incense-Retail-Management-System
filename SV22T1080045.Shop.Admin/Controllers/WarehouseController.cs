using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.Admin.Extensions.Authorizations;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.Abstractions.Contracts.Warehouse;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Models.ViewModels.Management;
using SV22T1080045.Shop.Models.ViewModels.Warehouse;

namespace SV22T1080045.Shop.Controllers
{
    [Authorize]
    public class WarehouseController : Controller
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public WarehouseController(IWarehouseService warehouseService, IProductService productService, IOrderService orderService)
        {
            _warehouseService = warehouseService;
            _productService = productService;
            _orderService = orderService;
        }

        public IActionResult Index(int inventoryPage = 1)
        {
            // Dashboard returns both inventory and customer debt totals, so require both.
            if (!User.HasAllPermissions(PermissionConstants.ViewInventory, PermissionConstants.ViewDebt))
                return Forbid();

            const int inventoryPageSize = 10;
            var currentPage = Math.Max(1, inventoryPage);
            var inventories = _warehouseService.ListStockInventories(currentPage, inventoryPageSize, out var inventoryTotalCount);
            var totalPages = Math.Max(1, (int)Math.Ceiling(inventoryTotalCount / (double)inventoryPageSize));
            if (currentPage > totalPages)
            {
                currentPage = totalPages;
                inventories = _warehouseService.ListStockInventories(currentPage, inventoryPageSize, out inventoryTotalCount);
            }
            var inventorySummary = _warehouseService.GetStockInventorySummary();
            var (lowStockItems, zeroStockItems) = _warehouseService.GetInventoryAlerts(5);
            var fromDate = DateTime.Today.AddDays(-30);
            var toDate = DateTime.Today.AddDays(1).AddTicks(-1);
            var recentTransactions = _warehouseService.ListRecentStockTransactions(8, fromDate, toDate);
            var topExports = _warehouseService.ListTopExportProducts(fromDate, toDate, 5);
            var transactionSummary = _warehouseService.GetStockDashboardData(fromDate, toDate);

            return View(new WarehouseDashboardViewModel
            {
                StockInventories = inventories.Select(item => new WarehouseInventoryRowViewModel
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName ?? $"Sản phẩm #{item.ProductId}",
                    CurrentQuantity = item.CurrentQuantity,
                    LowStockThreshold = item.LowStockThreshold,
                    EstimatedValue = item.TotalValue,
                    LastUpdated = item.LastUpdated
                }).ToList(),
                LowStockProducts = lowStockItems.Select(item => new WarehouseInventoryRowViewModel
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName ?? $"Sản phẩm #{item.ProductId}",
                    CurrentQuantity = item.CurrentQuantity,
                    LowStockThreshold = item.LowStockThreshold,
                    EstimatedValue = item.TotalValue,
                    LastUpdated = item.LastUpdated
                }).ToList(),
                OutOfStockProducts = zeroStockItems.Select(item => new WarehouseInventoryRowViewModel
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName ?? $"Sản phẩm #{item.ProductId}",
                    CurrentQuantity = item.CurrentQuantity,
                    LowStockThreshold = item.LowStockThreshold,
                    EstimatedValue = item.TotalValue,
                    LastUpdated = item.LastUpdated
                }).ToList(),
                RecentTransactions = recentTransactions.Take(8).Select(transaction => new WarehouseTransactionRowViewModel
                {
                    Id = transaction.Id,
                    ProductId = transaction.ProductId,
                    ProductName = transaction.ProductName ?? $"Sản phẩm #{transaction.ProductId}",
                    Quantity = transaction.Quantity,
                    TransactionType = transaction.TransactionType,
                    ReferenceNumber = transaction.ReferenceNumber,
                    TransactionDate = transaction.TransactionDate,
                    CreatedBy = transaction.CreatedBy
                }).ToList(),
                TopExportProducts = topExports.Select(item => new WarehouseTopExportViewModel
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName ?? $"Sản phẩm #{item.ProductId}",
                    Quantity = item.Quantity
                }).ToList(),
                LowStockCount = inventorySummary.LowStockCount,
                OutOfStockCount = inventorySummary.OutOfStockCount,
                InventoryValue = inventorySummary.InventoryValue,
                TotalStockQuantity = inventorySummary.TotalQuantity,
                InventoryPage = currentPage,
                InventoryPageSize = inventoryPageSize,
                InventoryTotalCount = inventoryTotalCount,
                InventoryTotalPages = totalPages,
                TransactionCountLast30Days = transactionSummary.TransactionCount,
                ImportCountLast30Days = transactionSummary.ImportCount,
                ExportCountLast30Days = transactionSummary.ExportCount,
                CanImportInventory = User.HasAnyPermission(PermissionConstants.ImportInventory, PermissionConstants.ManageSystem),
                CanExportInventory = User.HasAnyPermission(PermissionConstants.ExportInventory, PermissionConstants.ManageSystem)
            });
        }

        [Permission(PermissionConstants.ViewInventory)]
        public IActionResult StockTransactions(int? productId = null, DateTime? fromDate = null, DateTime? toDate = null, string transactionType = "Import", int? reimportOrderId = null)
        {
            var transactions = _warehouseService.ListStockTransactions(productId ?? 0, fromDate, toDate, transactionType);
            var products = _productService.ListProducts();

            return View(new StockTransactionPageViewModel
            {
                Transactions = transactions.Select(transaction => new StockTransactionRowViewModel
                {
                    Id = transaction.Id,
                    ProductId = transaction.ProductId,
                    ProductName = transaction.ProductName ?? $"Sản phẩm #{transaction.ProductId}",
                    Quantity = transaction.Quantity,
                    TransactionType = transaction.TransactionType,
                    UnitPrice = transaction.UnitPrice,
                    TotalAmount = transaction.TotalAmount,
                    ReferenceNumber = transaction.ReferenceNumber,
                    TransactionDate = transaction.TransactionDate,
                    CreatedBy = transaction.CreatedBy
                }).ToList(),
                ProductId = productId,
                FromDate = fromDate,
                ToDate = toDate,
                TransactionType = transactionType,
                ReimportOrderId = reimportOrderId,
                ReimportOrders = _warehouseService.ListOrdersForReimport().Select(order => new WarehouseOptionViewModel
                {
                    Value = order.OrderId.ToString(),
                    Text = $"Đơn #{order.OrderId} · {order.OrderDate:dd/MM/yyyy} · {order.CustomerName}",
                    Selected = order.OrderId == reimportOrderId
                }).ToList(),
                ReimportLines = reimportOrderId.HasValue
                    ? _warehouseService.ListReimportOrderLines(reimportOrderId.Value).Select(line => new ReimportOrderLineViewModel
                    {
                        OrderDetailId = line.OrderDetailId,
                        ProductId = line.ProductId,
                        ProductName = line.ProductName,
                        ExportedQuantity = line.ExportedQuantity,
                        ReturnedQuantity = line.ReturnedQuantity,
                        ReturnableQuantity = line.ReturnableQuantity,
                        UnitPrice = line.UnitPrice,
                        ImportPrice = line.ImportPrice
                    }).ToList()
                    : new List<ReimportOrderLineViewModel>(),
                AvailableProducts = products.Select(p => new WarehouseOptionViewModel
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.ProductName} (ID: {p.Id})"
                }).ToList()
            });
        }

        [Permission(PermissionConstants.ViewDebt)]
        public IActionResult CustomerDebts(int? customerId = null)
        {
            var debts = _warehouseService.ListCustomerDebts(customerId);
            return View(new CustomerDebtPageViewModel
            {
                Debts = debts.Select(debt => new CustomerDebtRowViewModel
                {
                    Id = debt.Id,
                    CustomerId = debt.CustomerId,
                    OrderCode = debt.OrderCode,
                    DebtAmount = debt.DebtAmount,
                    PaidAmount = debt.PaidAmount,
                    DueDate = debt.DueDate,
                    Status = debt.Status
                }).ToList(),
                CustomerId = customerId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStockTransaction(StockTransactionInputViewModel input)
        {
            var requiredPermission = input.TransactionType?.Trim().ToLowerInvariant() switch
            {
                "import" or "restock" => PermissionConstants.ImportInventory,
                "export" => PermissionConstants.ExportInventory,
                "adjustment" => PermissionConstants.AdjustInventory,
                _ => null
            };

            if (requiredPermission == null)
                return BadRequest("Loại giao dịch kho không hợp lệ.");

            if (!User.HasAnyPermission(requiredPermission, PermissionConstants.ManageSystem))
                return Forbid();

            if (input.Quantity == 0 || input.UnitPrice < 0 ||
                (!string.Equals(input.TransactionType, "Adjustment", StringComparison.OrdinalIgnoreCase) && input.Quantity < 0))
            {
                TempData["ErrorMessage"] = "Số lượng phải lớn hơn 0 và đơn giá không được âm.";
                return RedirectToAction(nameof(StockTransactions));
            }

            var transactionId = _warehouseService.AddStockTransaction(new StockTransactionRequest
            {
                ProductId = input.ProductId,
                Quantity = input.Quantity,
                TransactionType = input.TransactionType,
                UnitPrice = input.UnitPrice,
                ReferenceNumber = input.ReferenceNumber,
                Reason = input.Reason,
                TransactionDate = input.TransactionDate
            });
            if (transactionId > 0)
            {
                TempData["SuccessMessage"] = "Đã thêm giao dịch kho thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể thêm giao dịch kho.";
            }
            return RedirectToAction(nameof(StockTransactions));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(PermissionConstants.ImportInventory, PermissionConstants.ManageSystem)]
        public IActionResult AddReimportStockTransaction(ReimportStockInputViewModel input)
        {
            if (input.Quantity <= 0)
            {
                TempData["ErrorMessage"] = "Số lượng phải lớn hơn 0.";
                return RedirectToAction(nameof(StockTransactions), new { reimportOrderId = input.OrderId });
            }

            var succeeded = _warehouseService.AddReimportTransaction(new ReimportStockRequest
            {
                OrderId = input.OrderId,
                OrderDetailId = input.OrderDetailId,
                Quantity = input.Quantity,
                Reason = input.Reason?.Trim(),
                TransactionDate = input.TransactionDate
            }, out var error);

            TempData[succeeded ? "SuccessMessage" : "ErrorMessage"] = succeeded
                ? "Đã ghi nhận tái nhập và cập nhật tồn kho."
                : error;
            return RedirectToAction(nameof(StockTransactions), new { reimportOrderId = input.OrderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(PermissionConstants.ManageDebt, PermissionConstants.ManageSystem)]
        public IActionResult AddCustomerDebt(CustomerDebtInputViewModel input)
        {
            var debtId = _warehouseService.AddCustomerDebt(new CustomerDebtRequest
            {
                CustomerId = input.CustomerId,
                OrderCode = input.OrderCode,
                DebtAmount = input.DebtAmount,
                PaidAmount = input.PaidAmount,
                PaymentReference = input.PaymentReference,
                DueDate = input.DueDate,
                Notes = input.Notes
            });
            if (debtId > 0)
            {
                TempData["SuccessMessage"] = "Đã thêm công nợ thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể thêm công nợ.";
            }
            return RedirectToAction(nameof(CustomerDebts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(PermissionConstants.ProcessPayment, PermissionConstants.CollectPayment, PermissionConstants.ManageSystem)]
        public IActionResult ProcessPayment(int debtId, decimal paymentAmount, string? paymentReference)
        {
            var result = _warehouseService.ProcessDebtPayment(debtId, paymentAmount, reference: paymentReference);
            if (result)
            {
                TempData["SuccessMessage"] = "Đã xử lý thanh toán thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xử lý thanh toán.";
            }
            return RedirectToAction(nameof(CustomerDebts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(PermissionConstants.ManageDebt, PermissionConstants.ManageSystem)]
        public IActionResult CreateDebtFromOrder(int orderId)
        {
            var order = _orderService.GetOrder(orderId);
            if (order == null || order.PaymentMethod != 1 || order.Status != 4 || order.CustomerId <= 0)
            {
                TempData["ErrorMessage"] = "Chỉ có thể tạo công nợ cho đơn COD đã hoàn thành.";
                return RedirectToAction("Index", "Management");
            }

            var result = _warehouseService.CreateDebtFromOrder(orderId, order.CustomerId, order.FinalAmount);
            if (result)
            {
                TempData["SuccessMessage"] = "Đã tạo công nợ thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể tạo công nợ (có thể đã tồn tại).";
            }
            return RedirectToAction("Index", "Management");
        }
    }

}
