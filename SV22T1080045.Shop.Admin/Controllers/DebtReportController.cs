using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.BusinessLayers.Interfaces;


namespace SV22T1080045.Shop.Admin.Controllers
{
    [Authorize]
    public class DebtReportController : Controller
    {
        private readonly ICustomerDebtReportService _reportService;
        private readonly ICustomerDAL _customerDAL;

        public DebtReportController(ICustomerDebtReportService reportService, ICustomerDAL customerDAL)
        {
            _reportService = reportService;
            _customerDAL = customerDAL;
        }

        public IActionResult Index()
        {
            var overview = _reportService.GetDebtOverview();
            return View(overview);
        }

        public IActionResult Summary(DateTime? startDate, DateTime? endDate, int? customerId, string? status)
        {
            var summary = _reportService.GetDebtSummary(startDate, endDate, customerId, status);
            var customers = _customerDAL.ListForPermissionAssignment();
            ViewBag.Customers = customers;
            ViewBag.SelectedCustomerId = customerId;
            ViewBag.SelectedStatus = status;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            return View(summary);
        }

        public IActionResult Ledger(int customerId, DateTime? startDate, DateTime? endDate)
        {
            var customer = _customerDAL.GetById(customerId);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng.";
                return RedirectToAction(nameof(Summary));
            }

            var ledger = _reportService.GetDebtLedger(customerId, startDate, endDate);
            ViewBag.Customer = customer;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            return View(ledger);
        }

        public IActionResult Aging(DateTime? asOfDate, int? customerId)
        {
            var date = asOfDate ?? DateTime.Now;
            var aging = _reportService.GetDebtAging(date, customerId);
            var customers = _customerDAL.ListForPermissionAssignment();
            ViewBag.Customers = customers;
            ViewBag.SelectedCustomerId = customerId;
            ViewBag.AsOfDate = date.ToString("yyyy-MM-dd");
            return View(aging);
        }
    }
}
