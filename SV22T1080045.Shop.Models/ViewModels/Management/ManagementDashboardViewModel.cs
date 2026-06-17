using SV22T1080045.Shop.Models.ViewModels.Product;
using System.ComponentModel.DataAnnotations;

namespace SV22T1080045.Shop.Models.ViewModels.Management
{
    public class ManagementDashboardViewModel
    {
        public List<ManagementCategoryOptionViewModel> Categories { get; set; } = new();
        public List<ManagementUnitOptionViewModel> Units { get; set; } = new();
        public List<ManagementProductRowViewModel> AllProducts { get; set; } = new();
        public List<ManagementProductRowViewModel> Products { get; set; } = new();
        public ProductEditViewModel ProductForm { get; set; } = new();

        public string SearchValue { get; set; } = "";
        public int? CategoryId { get; set; }
        public string ProductStatus { get; set; } = "";
        public int ProductPage { get; set; } = 1;
        public int ProductPageSize { get; set; } = 10;
        public int ProductTotalCount { get; set; }
        public int ProductTotalPages { get; set; } = 1;
        public int? LastSavedProductId { get; set; }
        public string? LastSavedAction { get; set; }

        public string RevenuePeriod { get; set; } = "day";
        public DateTime RevenueDate { get; set; } = DateTime.Today;
        public DateTime RevenueRangeStart { get; set; } = DateTime.Today;
        public DateTime RevenueRangeEnd { get; set; } = DateTime.Today.AddDays(1);
        public ManagementMetricSummary RevenueSummary { get; set; } = new();
        public List<RevenuePointViewModel> RevenuePoints { get; set; } = new();

        public List<ManagementOrderViewModel> RecentOrders { get; set; } = new();
        public List<OrderStatusGroupViewModel> OrderStatusGroups { get; set; } = new();

        public List<CustomerManagementRowViewModel> Customers { get; set; } = new();
        public CustomerManagementRowViewModel? SelectedCustomer { get; set; }
        public List<CustomerPurchaseHistoryViewModel> SelectedCustomerHistory { get; set; } = new();
        public bool ShowFullCustomerHistory { get; set; }

        public ManagementVoucherInput VoucherForm { get; set; } = new();
        public List<ManagementVoucherViewModel> Vouchers { get; set; } = new();
        public string? VoucherMessage { get; set; }

        public string ReportPeriod { get; set; } = "month";
        public DateTime ReportDate { get; set; } = DateTime.Today;
        public DateTime ReportRangeStart { get; set; } = DateTime.Today;
        public DateTime ReportRangeEnd { get; set; } = DateTime.Today.AddDays(1);
        public ManagementMetricSummary ReportSummary { get; set; } = new();
        public List<RevenuePointViewModel> ReportRevenuePoints { get; set; } = new();
        public List<ProductSalesReportViewModel> ProductSalesReports { get; set; } = new();
        public List<CustomerManagementRowViewModel> TopCustomers { get; set; } = new();
    }

    public class ManagementCategoryOptionViewModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = "";
    }

    public class ManagementUnitOptionViewModel
    {
        public int Id { get; set; }
        public string UnitName { get; set; } = "";
    }

    public class ManagementProductRowViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = "";
        public int UnitId { get; set; }
        public int CategoryId { get; set; }
        public decimal ImportPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public string? Origin { get; set; }
        public string? OilContent { get; set; }
        public int? Quantity { get; set; }
        public int LowStockThreshold { get; set; } = 5;
        public int? SoldCount { get; set; }
    }

    public class ManagementMetricSummary
    {
        public decimal Revenue { get; set; }
        public decimal PreviousRevenue { get; set; }
        public int OrderCount { get; set; }
        public int CompletedOrderCount { get; set; }
        public int NewCustomerCount { get; set; }
        public int GuestOrderCount { get; set; }
        public decimal AverageOrderValue => OrderCount == 0 ? 0 : Revenue / OrderCount;
        public decimal RevenueChangePercent => PreviousRevenue == 0
            ? (Revenue > 0 ? 100 : 0)
            : (Revenue - PreviousRevenue) / PreviousRevenue * 100;
    }

    public class RevenuePointViewModel
    {
        public string Label { get; set; } = "";
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public decimal PercentOfMax { get; set; }
    }

    public class ManagementOrderViewModel
    {
        public int Id { get; set; }
        public string OrderCode => $"#TH{Id:D6}";
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public string CustomerName { get; set; } = "";
        public string CustomerPhone { get; set; } = "";
        public int ItemCount { get; set; }
        public bool IsGuest { get; set; }
    }

    public class OrderStatusGroupViewModel
    {
        public int Status { get; set; }
        public string StatusText { get; set; } = "";
        public string StatusClass { get; set; } = "";
        public List<ManagementOrderViewModel> Orders { get; set; } = new();
    }

    public class CustomerManagementRowViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = "Customer";
        public DateTime CreatedTime { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }

    public class CustomerPurchaseHistoryViewModel
    {
        public int OrderId { get; set; }
        public string OrderCode => $"#TH{OrderId:D6}";
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public int ItemCount { get; set; }
        public List<string> ProductNames { get; set; } = new();
    }

    public class ProductSalesReportViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public enum ManagementVoucherDiscountType
    {
        Percent = 1,
        FixedAmount = 2
    }

    public class ManagementVoucherViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
        public ManagementVoucherDiscountType DiscountType { get; set; } = ManagementVoucherDiscountType.Percent;
        public decimal DiscountValue { get; set; }
        public decimal MinOrderAmount { get; set; }
        public int? MaxUsage { get; set; }
        public int UsedCount { get; set; }
        public bool IsActive { get; set; }
    }

    public class ManagementVoucherInput
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã giảm giá.")]
        public string Code { get; set; } = "";

        public string Description { get; set; } = "";
        public ManagementVoucherDiscountType DiscountType { get; set; } = ManagementVoucherDiscountType.Percent;

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị giảm phải lớn hơn 0.")]
        public decimal DiscountValue { get; set; } = 10;

        public decimal? MaxDiscount { get; set; }
        public decimal MinOrderAmount { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int? MaxUsage { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
