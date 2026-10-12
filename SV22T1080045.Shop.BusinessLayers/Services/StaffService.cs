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
                    Message = "Nhân viên chỉ được cập nhật trạng thái từ chờ xử lý đến hoàn thành."
                };
            }

            if (!_staffDAL.UpdateOrderStatus(orderId, status))
                return new StaffOperationResult { Message = "Không tìm thấy đơn hàng cần cập nhật." };

            return new StaffOperationResult
            {
                Success = true,
                Message = $"Đã cập nhật {FormatOrderCode(orderId)} sang trạng thái {StatusText(status)}."
            };
        }

        public StaffOperationResult UpdateOrderStatusWithInventoryCheck(int orderId, int status)
        {
            if (status is < 1 or > 4)
            {
                return new StaffOperationResult
                {
                    Message = "Nhân viên chỉ được cập nhật trạng thái từ chờ xử lý đến hoàn thành."
                };
            }

            var (success, inventoryChecks) = _staffDAL.UpdateOrderStatusWithInventoryCheck(orderId, status);

            if (!success)
            {
                if (inventoryChecks.Any())
                {
                    var insufficientProducts = inventoryChecks.Where(c => !c.IsSufficient).ToList();
                    var message = $"Không thể xác nhận đơn {FormatOrderCode(orderId)}: ";
                    message += string.Join(", ", insufficientProducts.Select(c => $"{c.ProductName} (còn {c.AvailableQuantity}, cần {c.RequestedQuantity})"));
                    return new StaffOperationResult
                    {
                        Success = false,
                        Message = message,
                        InventoryChecks = inventoryChecks
                    };
                }
                return new StaffOperationResult { Message = "Không tìm thấy đơn hàng cần cập nhật." };
            }

            var resultMessage = $"Đã cập nhật {FormatOrderCode(orderId)} sang trạng thái {StatusText(status)}.";
            if (status == 2 && inventoryChecks.Any())
            {
                var stockInfo = string.Join(", ", inventoryChecks.Select(c => $"{c.ProductName}: -{c.RequestedQuantity}"));
                resultMessage += $" Đã trừ kho: {stockInfo}.";
            }

            return new StaffOperationResult
            {
                Success = true,
                Message = resultMessage,
                InventoryChecks = inventoryChecks
            };
        }

        private static string FormatOrderCode(int orderId) => $"#TH{orderId:D6}";

        private static string StatusText(int status) => status switch
        {
            1 => "Chờ xử lý",
            2 => "Đang chuẩn bị",
            3 => "Đang giao",
            4 => "Hoàn thành",
            _ => "Khác"
        };
    }
}
