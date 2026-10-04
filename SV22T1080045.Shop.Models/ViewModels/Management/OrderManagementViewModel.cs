namespace SV22T1080045.Shop.Models.ViewModels.Management
{
    public class OrderManagementViewModel
    {
        public List<ManagementOrderViewModel> RecentOrders { get; set; } = new();
        public List<OrderStatusGroupViewModel> OrderStatusGroups { get; set; } = new();
        public int? SelectedOrderId { get; set; }
        public ManagementOrderViewModel? SelectedOrder { get; set; }
        public List<ManagementOrderDetailViewModel> SelectedOrderDetails { get; set; } = new();
        public int OrderPage { get; set; } = 1;
        public int OrderPageSize { get; set; } = 10;
        public int OrderTotalCount { get; set; }
        public int OrderTotalPages { get; set; } = 1;
        public int? CurrentStatus { get; set; }
    }
}