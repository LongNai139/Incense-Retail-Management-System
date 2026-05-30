using System.Globalization;
using System.Text;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models.Reports;
using SV22T1080045.Shop.BusinessLayers.Interfaces;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class RevenueReportService : IRevenueReportService
    {
        private readonly IRevenueReportDAL _revenueReportDAL;

        public RevenueReportService(IRevenueReportDAL revenueReportDAL)
        {
            _revenueReportDAL = revenueReportDAL;
        }

        public RevenueReportData GetRevenueReport(string? period, DateTime? date)
        {
            var normalizedPeriod = NormalizePeriod(period);
            var selectedDate = (date ?? DateTime.Today).Date;
            var range = ResolveRange(normalizedPeriod, selectedDate);
            var previousStart = range.Start - (range.End - range.Start);

            var report = _revenueReportDAL.GetRevenueReport(range.Start, range.End, previousStart, range.Start);
            report.Period = normalizedPeriod;
            report.StartDate = range.Start;
            report.EndDate = range.End;
            report.RevenuePoints = BuildRevenuePoints(report.OrderRows, range.Start, range.End, normalizedPeriod);

            return report;
        }

        public byte[] ExportRevenueReportCsv(string? period, DateTime? date)
        {
            var report = GetRevenueReport(period, date);
            var culture = CultureInfo.GetCultureInfo("vi-VN");
            var builder = new StringBuilder();

            AppendRow(builder, "BAO CAO DOANH THU");
            AppendRow(builder, "Ky bao cao", PeriodText(report.Period));
            AppendRow(builder, "Tu ngay", report.StartDate.ToString("dd/MM/yyyy", culture));
            AppendRow(builder, "Den ngay", report.EndDate.AddDays(-1).ToString("dd/MM/yyyy", culture));
            AppendRow(builder, "Ngay xuat", DateTime.Now.ToString("dd/MM/yyyy HH:mm", culture));
            AppendRow(builder);

            AppendRow(builder, "Tong quan");
            AppendRow(builder, "Doanh thu", "Doanh thu ky truoc", "Tang truong", "Tong don", "Don hoan thanh", "Gia tri TB/don", "Khach moi", "Don guest");
            AppendRow(
                builder,
                report.Summary.Revenue.ToString("0.##", CultureInfo.InvariantCulture),
                report.Summary.PreviousRevenue.ToString("0.##", CultureInfo.InvariantCulture),
                report.Summary.RevenueChangePercent.ToString("0.##", CultureInfo.InvariantCulture) + "%",
                report.Summary.OrderCount.ToString(culture),
                report.Summary.CompletedOrderCount.ToString(culture),
                report.Summary.AverageOrderValue.ToString("0.##", CultureInfo.InvariantCulture),
                report.Summary.NewCustomerCount.ToString(culture),
                report.Summary.GuestOrderCount.ToString(culture));
            AppendRow(builder);

            AppendRow(builder, "Doanh thu theo moc");
            AppendRow(builder, "Moc", "Doanh thu", "So don");
            foreach (var point in report.RevenuePoints)
            {
                AppendRow(
                    builder,
                    point.Label,
                    point.Revenue.ToString("0.##", CultureInfo.InvariantCulture),
                    point.OrderCount.ToString(culture));
            }
            AppendRow(builder);

            AppendRow(builder, "Top san pham theo doanh thu");
            AppendRow(builder, "Ma SP", "San pham", "So luong", "So don", "Doanh thu");
            foreach (var row in report.ProductRows)
            {
                AppendRow(
                    builder,
                    row.ProductId.ToString(culture),
                    row.ProductName,
                    row.Quantity.ToString(culture),
                    row.OrderCount.ToString(culture),
                    row.Revenue.ToString("0.##", CultureInfo.InvariantCulture));
            }
            AppendRow(builder);

            AppendRow(builder, "Top khach hang trong ky");
            AppendRow(builder, "Ma KH", "Khach hang", "Dien thoai", "So don", "Doanh thu");
            foreach (var row in report.CustomerRows)
            {
                AppendRow(
                    builder,
                    row.CustomerId.ToString(culture),
                    row.CustomerName,
                    row.Phone,
                    row.OrderCount.ToString(culture),
                    row.Revenue.ToString("0.##", CultureInfo.InvariantCulture));
            }
            AppendRow(builder);

            AppendRow(builder, "Don hang trong ky");
            AppendRow(builder, "Ma don", "Ngay dat", "Khach hang", "Dien thoai", "Trang thai", "So san pham", "Tong tien");
            foreach (var row in report.OrderRows)
            {
                AppendRow(
                    builder,
                    row.OrderCode,
                    row.OrderDate.ToString("dd/MM/yyyy HH:mm", culture),
                    row.CustomerName,
                    row.CustomerPhone,
                    StatusText(row.Status),
                    row.ItemCount.ToString(culture),
                    row.TotalAmount.ToString("0.##", CultureInfo.InvariantCulture));
            }

            var bom = Encoding.UTF8.GetPreamble();
            var content = Encoding.UTF8.GetBytes(builder.ToString());
            var bytes = new byte[bom.Length + content.Length];
            Buffer.BlockCopy(bom, 0, bytes, 0, bom.Length);
            Buffer.BlockCopy(content, 0, bytes, bom.Length, content.Length);
            return bytes;
        }

        public string BuildRevenueReportFileName(string? period, DateTime? date)
        {
            var normalizedPeriod = NormalizePeriod(period);
            var selectedDate = (date ?? DateTime.Today).Date;
            return $"bao-cao-doanh-thu-{normalizedPeriod}-{selectedDate:yyyyMMdd}.csv";
        }

        private static List<RevenueReportPoint> BuildRevenuePoints(
            List<OrderRevenueReportRow> orders,
            DateTime start,
            DateTime end,
            string period)
        {
            var points = new List<RevenueReportPoint>();
            if (period == "day")
            {
                for (var hour = 0; hour < 24; hour += 3)
                {
                    var bucketStart = start.AddHours(hour);
                    var bucketEnd = bucketStart.AddHours(3);
                    var bucket = orders.Where(o => o.OrderDate >= bucketStart && o.OrderDate < bucketEnd).ToList();
                    points.Add(new RevenueReportPoint
                    {
                        Label = $"{hour:00}:00-{bucketEnd.Hour:00}:00",
                        Revenue = bucket.Sum(o => o.TotalAmount),
                        OrderCount = bucket.Count
                    });
                }
            }
            else
            {
                for (var day = start.Date; day < end.Date; day = day.AddDays(1))
                {
                    var nextDay = day.AddDays(1);
                    var bucket = orders.Where(o => o.OrderDate >= day && o.OrderDate < nextDay).ToList();
                    points.Add(new RevenueReportPoint
                    {
                        Label = day.ToString("dd/MM", CultureInfo.GetCultureInfo("vi-VN")),
                        Revenue = bucket.Sum(o => o.TotalAmount),
                        OrderCount = bucket.Count
                    });
                }
            }

            var maxRevenue = points.Max(p => (decimal?)p.Revenue) ?? 0;
            foreach (var point in points)
                point.PercentOfMax = maxRevenue <= 0 ? 0 : Math.Max(8, point.Revenue / maxRevenue * 100);

            return points;
        }

        private static (DateTime Start, DateTime End) ResolveRange(string period, DateTime date)
        {
            if (period == "week")
            {
                var diff = ((int)date.DayOfWeek + 6) % 7;
                var start = date.AddDays(-diff).Date;
                return (start, start.AddDays(7));
            }

            if (period == "month")
            {
                var start = new DateTime(date.Year, date.Month, 1);
                return (start, start.AddMonths(1));
            }

            return (date.Date, date.Date.AddDays(1));
        }

        private static string NormalizePeriod(string? period)
        {
            return period is "week" or "month" ? period : "day";
        }

        private static string PeriodText(string period) => period switch
        {
            "week" => "Theo tuan",
            "month" => "Theo thang",
            _ => "Theo ngay"
        };

        private static string StatusText(int status) => status switch
        {
            1 => "Cho xu ly",
            2 => "Dang chuan bi",
            3 => "Dang giao",
            4 => "Hoan thanh",
            -1 => "Da huy",
            _ => "Khac"
        };

        private static void AppendRow(StringBuilder builder, params string[] values)
        {
            builder.AppendLine(string.Join(",", values.Select(EscapeCsv)));
        }

        private static string EscapeCsv(string value)
        {
            value ??= "";
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
}
