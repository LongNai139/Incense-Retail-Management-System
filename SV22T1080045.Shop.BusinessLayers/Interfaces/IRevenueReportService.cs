using SV22T1080045.Shop.Abstractions.Models.Reports;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface IRevenueReportService
    {
        RevenueReportData GetRevenueReport(string? period, DateTime? date);
        byte[] ExportRevenueReportCsv(string? period, DateTime? date);
        string BuildRevenueReportFileName(string? period, DateTime? date);
    }
}
