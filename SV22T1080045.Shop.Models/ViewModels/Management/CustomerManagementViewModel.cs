namespace SV22T1080045.Shop.Models.ViewModels.Management
{
    public class CustomerManagementViewModel
    {
        public List<CustomerManagementRowViewModel> Customers { get; set; } = new();
        public CustomerManagementRowViewModel? SelectedCustomer { get; set; }
        public List<CustomerPurchaseHistoryViewModel> SelectedCustomerHistory { get; set; } = new();
        public bool ShowFullCustomerHistory { get; set; }
    }
}