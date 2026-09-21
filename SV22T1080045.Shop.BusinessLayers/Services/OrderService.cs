using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderDAL _orderDAL;
        private readonly IVoucherService? _voucherService;

        // Constructor không có voucher — giữ tương thích ngược
        public OrderService(IOrderDAL orderDAL)
        {
            _orderDAL = orderDAL;
        }

        // Constructor đầy đủ (khuyến nghị dùng)
        public OrderService(IOrderDAL orderDAL, IVoucherService voucherService)
        {
            _orderDAL = orderDAL;
            _voucherService = voucherService;
        }

        public int InitOrder(
            string shippingName,
            string shippingPhone,
            string shippingAddress,
            List<CartItem> cart,
            int customerId = 0,
            int paymentMethod = 1,
            string? note = null,
            string? voucherCode = null)
        {
            if (cart == null || cart.Count == 0)
                return 0;

            try
            {
                var total = cart.Sum(i => i.TotalPrice);

                // ── Xử lý voucher ─────────────────────────────────────────
                decimal discountAmount = 0;
                int? appliedVoucherId = null;
                string? appliedVoucherCode = null;

                if (!string.IsNullOrWhiteSpace(voucherCode) && _voucherService != null)
                {
                    var result = _voucherService.Apply(voucherCode.Trim(), total);
                    if (result.Success)
                    {
                        discountAmount = result.DiscountAmount;
                        appliedVoucherId = result.Voucher?.Id;
                        appliedVoucherCode = result.Voucher?.Code;
                    }
                    // Voucher không hợp lệ → bỏ qua, không chặn đơn
                }
                // ──────────────────────────────────────────────────────────

                var order = new Order
                {
                    CustomerId = customerId,
                    OrderDate = DateTime.Now,
                    TotalAmount = total,
                    Status = 1,
                    ShippingName = shippingName.Trim(),
                    ShippingPhone = shippingPhone.Trim(),
                    ShippingAddress = shippingAddress.Trim(),
                    IsDeleted = false,
                    Note = note?.Trim(),
                    PaymentMethod = paymentMethod,
                    PaymentStatus = 0,

                    // Voucher
                    VoucherId = appliedVoucherId,
                    VoucherCode = appliedVoucherCode,
                    DiscountAmount = discountAmount,
                    FinalAmount = total - discountAmount
                };

                var orderID = _orderDAL.AddOrder(order);
                if (orderID <= 0)
                    return 0;

                foreach (var item in cart)
                {
                    _orderDAL.AddOrderDetail(new OrderDetail
                    {
                        OrderId = orderID,
                        ProductId = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    });
                }

                // Tăng UsedCount SAU KHI đơn hàng tạo thành công
                if (appliedVoucherCode != null)
                    _voucherService!.Use(appliedVoucherCode);

                return orderID;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public Order? GetOrder(int orderID) => _orderDAL.GetOrder(orderID);

        public List<Order> GetCustomerOrders(int customerId) => _orderDAL.GetList(customerId);

        public List<OrderDetail> GetOrderDetails(int orderID) => _orderDAL.GetOrderDetails(orderID);

        public bool UpdateStatus(int orderID, int status) => _orderDAL.UpdateStatus(orderID, status);

        public bool MarkPaymentResult(
            int orderID,
            int paymentStatus,
            string? transactionNo,
            string? bankCode,
            string? responseCode,
            DateTime? paidAt)
        {
            return _orderDAL.UpdatePaymentResult(
                orderID,
                paymentStatus,
                transactionNo,
                bankCode,
                responseCode,
                paidAt);
        }
    }
}