using SV22T1080045.Shop.Abstractions.Models;
using SV22T1080045.Shop.DomainModels.Warehouse;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    public interface ICustomerDebtReportDAL
    {
        /// <summary>
        /// Lấy báo cáo tổng hợp công nợ theo khách hàng
        /// </summary>
        List<CustomerDebtSummaryModel> GetDebtSummary(DateTime? startDate, DateTime? endDate, int? customerId = null, string? status = null);

        /// <summary>
        /// Lấy sổ công nợ chi tiết theo khách hàng
        /// </summary>
        List<CustomerDebtTransaction> GetDebtLedger(int customerId, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Lấy báo cáo tuổi nợ theo khách hàng
        /// </summary>
        List<CustomerDebtAgingModel> GetDebtAging(DateTime asOfDate, int? customerId = null);

        /// <summary>
        /// Lấy thông tin tổng quan công nợ
        /// </summary>
        DebtOverviewModel GetDebtOverview(DateTime? startDate = null, DateTime? endDate = null);
    }
}
