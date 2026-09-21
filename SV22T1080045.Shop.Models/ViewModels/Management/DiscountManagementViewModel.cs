namespace SV22T1080045.Shop.Models.ViewModels.Management
{
    public class DiscountManagementViewModel
    {
        public ManagementVoucherInput VoucherForm { get; set; } = new();
        public List<ManagementVoucherViewModel> Vouchers { get; set; } = new();
        public string? VoucherMessage { get; set; }
    }
}