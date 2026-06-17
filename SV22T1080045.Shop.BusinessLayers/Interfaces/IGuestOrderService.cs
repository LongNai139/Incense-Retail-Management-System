using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface IGuestOrderService
    {
        void SaveGuestOrder(int orderId, string phone);
        List<Order> GetOrdersByPhone(string phone);
        bool MergeToCustomer(string phone, int customerId);
    }
}
