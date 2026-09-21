namespace SV22T1080045.Shop.Abstractions.Models.Reports
{
    public class RevenueReportData
    {
        public string Period { get; set; } = "day";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public RevenueReportSummary Summary { get; set; } = new();
        public List<RevenueReportPoint> RevenuePoints { get; set; } = new();
        public List<ProductRevenueReportRow> ProductRows { get; set; } = new();
        public List<CustomerRevenueReportRow> CustomerRows { get; set; } = new();
        public List<OrderRevenueReportRow> OrderRows { get; set; } = new();
    }

    public class RevenueReportSummary
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

    public class RevenueReportPoint
    {
        public string Label { get; set; } = "";
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public decimal PercentOfMax { get; set; }
    }

    public class ProductRevenueReportRow
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class CustomerRevenueReportRow
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = "";
        public string Phone { get; set; } = "";
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class OrderRevenueReportRow
    {
        public int OrderId { get; set; }
        public string OrderCode => $"#TH{OrderId:D6}";
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; } = "";
        public string CustomerPhone { get; set; } = "";
        public int Status { get; set; }
        public int ItemCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
