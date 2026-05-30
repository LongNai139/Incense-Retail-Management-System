using SV22T1080045.Shop.Abstractions.Models.Management;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    public interface IManagementDAL
    {
        bool VoucherCodeExists(string code, int exceptId);
        int AddVoucher(Voucher voucher);
        Voucher? GetVoucher(int id);
        bool SetVoucherActive(int id, bool isActive);
        List<Voucher> ListVouchers(int take);
        List<ManagementOrderData> ListRecentOrders(int take);
        List<CustomerManagementData> ListCustomers();
        List<CustomerPurchaseHistoryData> ListCustomerHistory(int customerId, int take);
        ManagementOrderDetailsData? GetOrderDetails(int orderId);
    }
}
