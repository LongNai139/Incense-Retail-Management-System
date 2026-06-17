using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models.Staff;
using SV22T1080045.Shop.BusinessLayers.Interfaces;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffDAL _staffDAL;

        public StaffService(IStaffDAL staffDAL)
        {
            _staffDAL = staffDAL;
        }

        public StaffDashboardData GetDashboardData() => _staffDAL.GetDashboardData();

        public StaffOperationResult UpdateOrderStatus(int orderId, int status)
        {
            if (status is < 1 or > 4)
            {
                return new StaffOperationResult
                {
                    Message = "Nhan vien chi duoc cap nhat trang thai tu cho xu ly den hoan thanh."
                };
            }

            if (!_staffDAL.UpdateOrderStatus(orderId, status))
                return new StaffOperationResult { Message = "Khong tim thay don hang can cap nhat." };

            return new StaffOperationResult
            {
                Success = true,
                Message = $"Da cap nhat {FormatOrderCode(orderId)} sang trang thai {StatusText(status)}."
            };
        }

        private static string FormatOrderCode(int orderId) => $"#TH{orderId:D6}";

        private static string StatusText(int status) => status switch
        {
            1 => "Cho xu ly",
            2 => "Dang chuan bi",
            3 => "Dang giao",
            4 => "Hoan thanh",
            _ => "Khac"
        };
    }
}
