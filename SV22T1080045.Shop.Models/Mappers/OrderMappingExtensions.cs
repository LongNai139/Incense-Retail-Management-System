using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Models.ViewModels.Checkout;
using SV22T1080045.Shop.Models.ViewModels.Orders;

namespace SV22T1080045.Shop.Models.Mappers
{
    public static class OrderMappingExtensions
    {
        public static OrderTrackViewModel ToOrderTrackViewModel(this Order order, IEnumerable<OrderDetail> details)
        {
            return new OrderTrackViewModel
            {
                OrderId = order.Id,
                OrderCode = OrderStatusLabels.FormatOrderCode(order.Id),
                OrderDate = order.OrderDate,
                Status = order.Status,
                StatusText = OrderStatusLabels.GetStatusText(order.Status),
                PaymentStatus = order.PaymentStatus,
                PaymentStatusText = OrderStatusLabels.GetPaymentStatusText(order.PaymentStatus),
                PaymentMethodText = OrderStatusLabels.GetPaymentMethodText(order.PaymentMethod),
                ShippingName = order.ShippingName ?? "",
                ShippingPhone = order.ShippingPhone ?? "",
                ShippingAddress = order.ShippingAddress ?? "",
                TotalAmount = order.TotalAmount,
                DiscountAmount = order.DiscountAmount,
                FinalAmount = order.FinalAmount > 0 ? order.FinalAmount : order.TotalAmount - order.DiscountAmount,
                VoucherCode = order.VoucherCode,
                Timeline = OrderStatusLabels.BuildTimeline(order.Status),
                Details = details.Select(d => new CheckoutOrderDetailViewModel
                {
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice
                }).ToList()
            };
        }

        public static OrderListItemViewModel ToOrderListItem(
            this Order order,
            int itemCount,
            string trackUrl)
        {
            return new OrderListItemViewModel
            {
                OrderId = order.Id,
                OrderCode = OrderStatusLabels.FormatOrderCode(order.Id),
                OrderDate = order.OrderDate,
                Status = order.Status,
                StatusText = OrderStatusLabels.GetStatusText(order.Status),
                FinalAmount = order.FinalAmount > 0 ? order.FinalAmount : order.TotalAmount - order.DiscountAmount,
                ItemCount = itemCount,
                TrackUrl = trackUrl,
                Timeline = OrderStatusLabels.BuildTimeline(order.Status)
            };
        }
    }
}
