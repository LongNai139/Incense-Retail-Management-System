using SV22T1080045.Shop.Abstractions.Models.Staff;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface IStaffService
    {
        /// <summary>Chỉ dùng cho trang Index (dashboard).</summary>
        StaffDashboardData GetDashboardData();

        List<StaffOrderData> GetTodayOrders(int take, string? keyword = null, int? status = null, DateTime? orderBefore = null);
        StaffProductPageData QueryProducts(string? keyword, int page, int pageSize);
        List<StaffCustomerData> GetRecentCustomers(int take);

        StaffOperationResult UpdateOrderStatus(int orderId, int status);
        StaffOperationResult UpdateOrderStatusWithInventoryCheck(int orderId, int status);
    }
}
