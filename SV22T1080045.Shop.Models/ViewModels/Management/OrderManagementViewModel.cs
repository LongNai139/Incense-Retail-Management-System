namespace SV22T1080045.Shop.Models.ViewModels.Management
{
    public class OrderManagementViewModel
    {
        public List<ManagementOrderViewModel> RecentOrders { get; set; } = new();
        public List<OrderStatusGroupViewModel> OrderStatusGroups { get; set; } = new();
    }
}