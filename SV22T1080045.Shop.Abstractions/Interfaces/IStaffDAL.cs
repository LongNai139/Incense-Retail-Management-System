using SV22T1080045.Shop.Abstractions.Models.Staff;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    public interface IStaffDAL
    {
        StaffDashboardData GetDashboardData();
        bool UpdateOrderStatus(int orderId, int status);
    }
}
