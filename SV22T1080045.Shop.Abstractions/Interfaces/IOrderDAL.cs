using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    public interface IOrderDAL
    {
        int AddOrder(Order data);
        void AddOrderDetail(OrderDetail data);
        Order? GetOrder(int orderID);
        List<Order> GetList(int customerId);
        List<OrderDetail> GetOrderDetails(int orderID);
        bool UpdateStatus(int orderID, int status);
        bool UpdatePaymentResult(
            int orderID,
            int paymentStatus,
            string? transactionNo,
            string? bankCode,
            string? responseCode,
            DateTime? paidAt);
    }
}
