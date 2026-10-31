namespace SV22T1080045.Shop.Models.ViewModels.Management
{
    public class ReportManagementViewModel
    {
        public string ReportPeriod { get; set; } = "month";
        public DateTime ReportDate { get; set; } = DateTime.Today;
        public DateTime ReportRangeStart { get; set; } = DateTime.Today;
        public DateTime ReportRangeEnd { get; set; } = DateTime.Today.AddDays(1);
        public ManagementMetricSummary ReportSummary { get; set; } = new();
        public List<RevenuePointViewModel> ReportRevenuePoints { get; set; } = new();
        public List<ReportOrderStatusViewModel> OrderStatusBreakdown { get; set; } = new();
        public int TotalOrderCount { get; set; }
        public List<ProductSalesReportViewModel> ProductSalesReports { get; set; } = new();
        public List<CustomerManagementRowViewModel> TopCustomers { get; set; } = new();
        public List<ManagementCategoryOptionViewModel> Categories { get; set; } = new();
    }

    public class ReportOrderStatusViewModel
    {
        public int Status { get; set; }
        public string Label { get; set; } = "";
        public int Count { get; set; }
        public decimal SharePercent { get; set; }
        public string Color { get; set; } = "#aa997b";
    }
}
