using SV22T1080045.Shop.DataLayers;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers
{
    // ── Interface ─────────────────────────────────────────────────────────────
    /// <summary>
    /// Xử lý logic đơn hàng của khách vãng lai (guest).
    /// Không biết SMS hay OTP là gì — chỉ biết lưu và tra cứu đơn theo SĐT.
    /// </summary>
    public interface IGuestOrderService
    {
        /// <summary>Gọi ngay sau khi đặt hàng thành công</summary>
        void SaveGuestOrder(int orderId, string phone);

        /// <summary>Lấy lịch sử đơn theo SĐT (sau khi OtpService đã xác thực)</summary>
        List<Order> GetOrdersByPhone(string phone);

        /// <summary>Gộp lịch sử đơn guest vào tài khoản khi khách tạo tài khoản</summary>
        bool MergeToCustomer(string phone, int customerId);
    }

    // ── Implementation ────────────────────────────────────────────────────────
    public class GuestOrderService : IGuestOrderService
    {
        private readonly IGuestOrderDAL _dal;

        public GuestOrderService(IGuestOrderDAL dal)
        {
            _dal = dal;
        }

        public void SaveGuestOrder(int orderId, string phone)
            => _dal.Save(orderId, phone);

        public List<Order> GetOrdersByPhone(string phone)
            => _dal.GetOrdersByPhone(phone);

        public bool MergeToCustomer(string phone, int customerId)
            => _dal.MergeToCustomer(phone, customerId);
    }
}