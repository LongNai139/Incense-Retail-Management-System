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
        public List<ProductSalesReportViewModel> ProductSalesReports { get; set; } = new();
        public List<CustomerManagementRowViewModel> TopCustomers { get; set; } = new();
    }
}