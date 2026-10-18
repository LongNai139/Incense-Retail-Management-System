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

        public StaffDashboardData GetDashboardData()
        {
            var now = DateTime.Now;
            var orderData = _staffDAL.GetOrderDashboardData(now);
            var lowStockProducts = _staffDAL.GetLowStockProducts(5, out var lowStockCount);
            return new StaffDashboardData
            {
                TodayOrders = _staffDAL.GetTodayOrders(4, status: 1, orderBefore: now.AddHours(-2)),
                RecentOrders = orderData.RecentOrders,
                PendingTooLongOrders = orderData.PendingTooLongOrders,
                FailedDeliveryOrders = orderData.FailedDeliveryOrders,
                LowStockProducts = lowStockProducts,
                Customers = _staffDAL.GetRecentCustomers(20),
                NewOrdersToday = orderData.NewOrdersToday,
                ProcessingOrders = orderData.ProcessingOrders,
                OrdersToDeliver = orderData.OrdersToDeliver,
                LowStockCount = lowStockCount
            };
        }

        public List<StaffOrderData> GetTodayOrders(int take, string? keyword = null, int? status = null, DateTime? orderBefore = null)
            => _staffDAL.GetTodayOrders(Math.Clamp(take, 1, 500), keyword?.Trim(), status, orderBefore);

        public StaffProductPageData QueryProducts(string? keyword, int page, int pageSize)
        {
            pageSize = Math.Clamp(pageSize, 1, 100);
            return _staffDAL.QueryProducts(keyword?.Trim(), Math.Max(1, page), pageSize);
        }

        public List<StaffCustomerData> GetRecentCustomers(int take) => _staffDAL.GetRecentCustomers(take);

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
                return new StaffOperationResult
                {
                    Message = "Không cập nhật được đơn: không tìm thấy đơn hôm nay, hoặc trạng thái mới không hợp lệ (chỉ được chuyển tiếp, không quay lại bước trước)."
                };
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
