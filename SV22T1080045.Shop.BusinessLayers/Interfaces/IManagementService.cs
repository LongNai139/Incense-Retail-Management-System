using SV22T1080045.Shop.Abstractions.Models.Management;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface IManagementService
    {
        VoucherSaveResult CreateVoucher(Voucher voucher);
        VoucherToggleResult ToggleVoucher(int id);
        List<Voucher> ListVouchers(int take = 10);
        List<ManagementOrderData> ListRecentOrders(int take = 10);
        List<CustomerManagementData> ListCustomers();
        List<CustomerPurchaseHistoryData> ListCustomerHistory(int customerId, int take);
        ManagementOrderDetailsData? GetOrderDetails(int orderId);
    }
}
