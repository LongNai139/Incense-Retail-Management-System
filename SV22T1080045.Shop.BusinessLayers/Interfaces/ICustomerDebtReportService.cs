using SV22T1080045.Shop.Abstractions.Models;
using SV22T1080045.Shop.DomainModels.Warehouse;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface ICustomerDebtReportService
    {
        List<CustomerDebtSummaryModel> GetDebtSummary(DateTime? startDate, DateTime? endDate, int? customerId = null, string? status = null);
        List<CustomerDebtTransaction> GetDebtLedger(int customerId, DateTime? startDate = null, DateTime? endDate = null);
        List<CustomerDebtAgingModel> GetDebtAging(DateTime asOfDate, int? customerId = null);
        DebtOverviewModel GetDebtOverview(DateTime? startDate = null, DateTime? endDate = null);
    }
}
