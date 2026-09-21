using SV22T1080045.Shop.Models.ViewModels.Checkout;

namespace SV22T1080045.Shop.Models.ViewModels.Orders
{
    public class OrderTrackViewModel
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; } = "";
        public int PaymentStatus { get; set; }
        public string PaymentStatusText { get; set; } = "";
        public string PaymentMethodText { get; set; } = "";
        public string ShippingName { get; set; } = "";
        public string ShippingPhone { get; set; } = "";
        public string ShippingAddress { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string? VoucherCode { get; set; }
        public bool IsCancelled => Status == -1;
        public IReadOnlyList<OrderTimelineStepViewModel> Timeline { get; set; } = Array.Empty<OrderTimelineStepViewModel>();
        public List<CheckoutOrderDetailViewModel> Details { get; set; } = new();
        public bool CanPoll => Status is not (4 or -1);
    }

    public class OrderListItemViewModel
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; } = "";
        public decimal FinalAmount { get; set; }
        public int ItemCount { get; set; }
        public string TrackUrl { get; set; } = "";
        public IReadOnlyList<OrderTimelineStepViewModel> Timeline { get; set; } = Array.Empty<OrderTimelineStepViewModel>();
        public bool CanTrack => Status is not (4 or -1);
        public bool CanReorder => Status == 4;
    }

    public class MyOrdersViewModel
    {
        public List<OrderListItemViewModel> Orders { get; set; } = new();
        public bool IsGuestLookup { get; set; }
        public string LookupPhone { get; set; } = "";
        public IReadOnlyList<OrderListItemViewModel> ActiveOrders => Orders.Where(o => o.CanTrack).ToList();
        public IReadOnlyList<OrderListItemViewModel> CompletedOrders => Orders.Where(o => o.Status == 4).ToList();
        public IReadOnlyList<OrderListItemViewModel> ClosedOrders => Orders.Where(o => o.Status == -1).ToList();
    }
}
