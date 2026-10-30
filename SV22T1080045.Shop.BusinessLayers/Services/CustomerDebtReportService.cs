using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels.Warehouse;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class CustomerDebtReportService : ICustomerDebtReportService
    {
        private readonly ICustomerDebtReportDAL _reportDAL;

        public CustomerDebtReportService(ICustomerDebtReportDAL reportDAL)
        {
            _reportDAL = reportDAL;
        }

        public List<CustomerDebtSummaryModel> GetDebtSummary(DateTime? startDate, DateTime? endDate, int? customerId = null, string? status = null)
        {
            return _reportDAL.GetDebtSummary(startDate, endDate, customerId, status);
        }

        public List<CustomerDebtTransaction> GetDebtLedger(int customerId, DateTime? startDate = null, DateTime? endDate = null)
        {
            return _reportDAL.GetDebtLedger(customerId, startDate, endDate);
        }

        public List<CustomerDebtAgingModel> GetDebtAging(DateTime asOfDate, int? customerId = null)
        {
            return _reportDAL.GetDebtAging(asOfDate, customerId);
        }

        public DebtOverviewModel GetDebtOverview(DateTime? startDate = null, DateTime? endDate = null)
        {
            return _reportDAL.GetDebtOverview(startDate, endDate);
        }
    }
}
