using SV22T1080045.Shop.Abstractions.Models.Staff;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    public interface IStaffDAL
    {
        bool UpdateOrderStatus(int orderId, int status);
        StaffOrderDashboardData GetOrderDashboardData(DateTime now);
        List<StaffProductData> GetLowStockProducts(int take, out int totalCount);
        List<StaffOrderData> GetTodayOrders(int take, string? keyword = null, int? status = null, DateTime? orderBefore = null);
        StaffProductPageData QueryProducts(string? keyword, int page, int pageSize);
        List<StaffCustomerData> GetRecentCustomers(int take);
        (bool Success, List<InventoryCheckResult> InventoryChecks) UpdateOrderStatusWithInventoryCheck(int orderId, int status);
    }
}
