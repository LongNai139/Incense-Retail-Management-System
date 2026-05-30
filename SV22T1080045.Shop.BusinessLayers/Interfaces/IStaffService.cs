using SV22T1080045.Shop.Abstractions.Models.Staff;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface IStaffService
    {
        StaffDashboardData GetDashboardData();
        StaffOperationResult UpdateOrderStatus(int orderId, int status);
    }
}
