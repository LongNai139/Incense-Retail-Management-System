using SV22T1080045.Shop.BusinessLayers;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface IOrderService
    {
        int InitOrder(string shippingName, string shippingPhone, string shippingAddress, List<CartItem> cart, int customerId = 0);
        Order? GetOrder(int orderID);
        List<Order> GetCustomerOrders(int customerId);
        List<OrderDetail> GetOrderDetails(int orderID);
        bool UpdateStatus(int orderID, int status);
    }
}
