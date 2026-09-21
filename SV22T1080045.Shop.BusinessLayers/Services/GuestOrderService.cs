
using SV22T1080045.Shop.Abstractions;
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
            _guestOrderDAL.Save(orderId, PhoneNumberHelper.NormalizeVietnameseMobile(phone));
        }

        public List<Order> GetOrdersByPhone(string phone)
        {
            return _guestOrderDAL.GetOrdersByPhone(PhoneNumberHelper.NormalizeVietnameseMobile(phone));
        }

        public bool MergeToCustomer(string phone, int customerId)
        {
            return _guestOrderDAL.MergeToCustomer(PhoneNumberHelper.NormalizeVietnameseMobile(phone), customerId);
        }
    }
}
