using SV22T1080045.Shop.Abstractions.Models.Reports;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    public interface IRevenueReportDAL
    {
        RevenueReportData GetRevenueReport(DateTime start, DateTime end, DateTime previousStart, DateTime previousEnd);
    }
}
