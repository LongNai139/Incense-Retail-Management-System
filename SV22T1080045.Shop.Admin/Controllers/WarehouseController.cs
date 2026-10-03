using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels.Warehouse;
using SV22T1080045.Shop.Models.ViewModels.Management;

namespace SV22T1080045.Shop.Controllers
{
    [Authorize]
    public class WarehouseController : Controller
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IProductService _productService;

        public WarehouseController(IWarehouseService warehouseService, IProductService productService)
        {
            _warehouseService = warehouseService;
            _productService = productService;
        }

        public IActionResult Index()
        {
            var inventories = _warehouseService.ListStockInventories();
            var debts = _warehouseService.ListCustomerDebts(null);
            
            return View(new WarehouseDashboardViewModel
            {
                StockInventories = inventories,
                CustomerDebts = debts,
                LowStockCount = inventories.Count(i => i.IsLowStock),
                OutOfStockCount = inventories.Count(i => i.IsOutOfStock),
                TotalDebt = debts.Sum(d => d.RemainingAmount),
                OverdueDebtCount = debts.Count(d => d.IsOverdue)
            });
        }

        public IActionResult StockTransactions(int? productId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var transactions = _warehouseService.ListStockTransactions(productId ?? 0, fromDate, toDate);
            var products = _productService.ListProducts();
            
            return View(new StockTransactionViewModel
            {
                Transactions = transactions,
                ProductId = productId,
                FromDate = fromDate,
                ToDate = toDate,
                AvailableProducts = products.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.ProductName} (ID: {p.Id})"
                }).ToList()
            });
        }

        public IActionResult CustomerDebts(int? customerId = null)
        {
            var debts = _warehouseService.ListCustomerDebts(customerId);
            return View(new CustomerDebtViewModel
            {
                Debts = debts,
                CustomerId = customerId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStockTransaction(StockTransaction transaction)
        {
            var transactionId = _warehouseService.AddStockTransaction(transaction);
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
        public IActionResult AddCustomerDebt(CustomerDebt debt)
        {
            debt.OrderId = null; // We're using OrderCode instead
            debt.OrderAmount = debt.DebtAmount; // Set order amount equal to debt amount initially
            
            var debtId = _warehouseService.AddCustomerDebt(debt);
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
        public IActionResult ProcessPayment(int debtId, decimal paymentAmount)
        {
            var result = _warehouseService.ProcessDebtPayment(debtId, paymentAmount);
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
    }

    public class WarehouseDashboardViewModel
    {
        public List<StockInventory> StockInventories { get; set; } = new();
        public List<CustomerDebt> CustomerDebts { get; set; } = new();
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public decimal TotalDebt { get; set; }
        public int OverdueDebtCount { get; set; }
    }

    public class StockTransactionViewModel
    {
        public List<StockTransaction> Transactions { get; set; } = new();
        public int? ProductId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<SelectListItem> AvailableProducts { get; set; } = new();
    }

    public class CustomerDebtViewModel
    {
        public List<CustomerDebt> Debts { get; set; } = new();
        public int? CustomerId { get; set; }
    }
}