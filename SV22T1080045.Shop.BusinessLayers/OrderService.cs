using SV22T1080045.Shop.DataLayers;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers
{
    // ── Interface (để CheckoutController inject được) ──────────────────────
    public interface IOrderService
    {
        /// <summary>Tạo đơn hàng mới, trả về OrderID vừa tạo (0 = thất bại)</summary>
        int InitOrder(string shippingName, string shippingPhone,
                      string shippingAddress, List<CartItem> cart,
                      int customerId = 0);

        /// <summary>Lấy 1 đơn hàng theo ID</summary>
        Order? GetOrder(int orderID);

        /// <summary>Lấy danh sách đơn hàng của 1 khách hàng (member)</summary>
        List<Order> GetCustomerOrders(int customerId);

        /// <summary>Lấy danh sách chi tiết sản phẩm trong đơn</summary>
        List<OrderDetail> GetOrderDetails(int orderID);

        /// <summary>Cập nhật trạng thái đơn hàng</summary>
        bool UpdateStatus(int orderID, int status);
    }

    // ── Implementation ─────────────────────────────────────────────────────
    public class OrderService : IOrderService
    {
        private readonly OrderDAL _orderDAL;

        public OrderService(OrderDAL orderDAL)
        {
            _orderDAL = orderDAL;
        }

        public int InitOrder(string shippingName, string shippingPhone,
                             string shippingAddress, List<CartItem> cart,
                             int customerId = 0)
        {
            try
            {
                if (cart == null || cart.Count == 0) return 0;

                var order = new Order
                {
                    CustomerId = customerId,   // 0 nếu là guest
                    OrderDate = DateTime.Now,
                    TotalAmount = cart.Sum(i => i.TotalPrice),
                    Status = 1,            // 1 = Mới đặt
                    ShippingName = shippingName,
                    ShippingPhone = shippingPhone,
                    ShippingAddress = shippingAddress,
                };

                int orderID = _orderDAL.AddOrder(order);
                if (orderID <= 0) return 0;

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

                return orderID;
            }
            catch
            {
                return 0;
            }
        }

        public Order? GetOrder(int orderID)
            => _orderDAL.GetOrder(orderID);

        public List<Order> GetCustomerOrders(int customerId)
            => _orderDAL.GetList(customerId);

        public List<OrderDetail> GetOrderDetails(int orderID)
            => _orderDAL.GetOrderDetails(orderID);

        public bool UpdateStatus(int orderID, int status)
            => _orderDAL.UpdateStatus(orderID, status);
    }
}