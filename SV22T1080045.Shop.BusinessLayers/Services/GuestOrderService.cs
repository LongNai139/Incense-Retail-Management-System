
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class GuestOrderService : IGuestOrderService
    {
        private readonly IGuestOrderDAL _guestOrderDAL;

        public GuestOrderService(IGuestOrderDAL guestOrderDAL)
        {
            _guestOrderDAL = guestOrderDAL;
        }

        public void SaveGuestOrder(int orderId, string phone)
        {
            _guestOrderDAL.Save(orderId, phone.Trim());
        }

        public List<Order> GetOrdersByPhone(string phone)
        {
            return _guestOrderDAL.GetOrdersByPhone(phone.Trim());
        }

        public bool MergeToCustomer(string phone, int customerId)
        {
            return _guestOrderDAL.MergeToCustomer(phone.Trim(), customerId);
        }
    }
}
