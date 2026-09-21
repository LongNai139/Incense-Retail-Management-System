using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    public interface IGuestOrderDAL
    {
        void Save(int orderId, string phone);
        List<Order> GetOrdersByPhone(string phone);
        bool MergeToCustomer(string phone, int customerId);
    }
}
