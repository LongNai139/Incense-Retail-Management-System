using SV22T1080045.Shop.Abstractions.Models.Operational;
using SV22T1080045.Shop.BusinessLayers.Interfaces;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class OperationalAlertService : IOperationalAlertService
    {
        private static readonly TimeSpan LateOrderThreshold = TimeSpan.FromHours(2);

        private readonly IStaffService _staffService;

        public OperationalAlertService(IStaffService staffService)
        {
            _staffService = staffService;
        }

        public OperationalAlertFeed GetFeed(DateTime? since)
        {
            var watermark = since ?? DateTime.Today;
            var now = DateTime.Now;
            var data = _staffService.GetDashboardData();
            var alerts = new List<OperationalAlertItem>();

            foreach (var order in data.Orders)
            {
                var orderCode = $"#TH{order.Id:D6}";

                if (order.Status == 1 && order.OrderDate > watermark)
                {
                    alerts.Add(new OperationalAlertItem
                    {
                        Key = $"new-{order.Id}",
                        Kind = "new_order",
                        Title = "Đơn hàng mới",
                        Message = $"{orderCode} – {order.CustomerName} cần xác nhận",
                        TargetAnchor = $"order-{order.Id}",
                        OccurredAt = order.OrderDate
                    });
                }

                if (order.Status == 1 && order.OrderDate <= now - LateOrderThreshold)
                {
                    alerts.Add(new OperationalAlertItem
                    {
                        Key = $"late-{order.Id}",
                        Kind = "late_order",
                        Title = "Đơn chậm xử lý",
                        Message = $"{orderCode} đã chờ hơn 2 giờ – {order.CustomerName}",
                        TargetAnchor = $"order-{order.Id}",
                        OccurredAt = order.OrderDate.Add(LateOrderThreshold)
                    });
                }

                if (order.Status == -1 && order.OrderDate > watermark)
                {
                    alerts.Add(new OperationalAlertItem
                    {
                        Key = $"cancel-{order.Id}",
                        Kind = "cancelled",
                        Title = "Đơn bị hủy / giao thất bại",
                        Message = $"{orderCode} – {order.CustomerName}",
                        TargetAnchor = $"order-{order.Id}",
                        OccurredAt = order.OrderDate
                    });
                }
            }

            foreach (var product in data.Products.Where(p => p.Quantity > 0 && p.Quantity < 5))
            {
                alerts.Add(new OperationalAlertItem
                {
                    Key = $"stock-{product.Id}",
                    Kind = "low_stock",
                    Title = "Sắp hết hàng",
                    Message = $"{product.ProductName} còn {product.Quantity} {product.UnitName}",
                    TargetAnchor = "products",
                    OccurredAt = now
                });
            }

            return new OperationalAlertFeed
            {
                ServerTime = now,
                Alerts = alerts
                    .OrderByDescending(a => a.OccurredAt)
                    .Take(25)
                    .ToList()
            };
        }
    }
}
