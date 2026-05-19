
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderDAL _orderDAL;

        public OrderService(IOrderDAL orderDAL)
        {
            _orderDAL = orderDAL;
        }

        public int InitOrder(string shippingName, string shippingPhone, string shippingAddress, List<CartItem> cart, int customerId = 0)
        {
            if (cart == null || cart.Count == 0)
                return 0;

            try
            {
                var order = new Order
                {
                    CustomerId = customerId,
                    OrderDate = DateTime.Now,
                    TotalAmount = cart.Sum(i => i.TotalPrice),
                    Status = 1,
                    ShippingName = shippingName.Trim(),
                    ShippingPhone = shippingPhone.Trim(),
                    ShippingAddress = shippingAddress.Trim()
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

                return orderID;
            }
            catch
            {
                return 0;
            }
        }

        public Order? GetOrder(int orderID) => _orderDAL.GetOrder(orderID);

        public List<Order> GetCustomerOrders(int customerId) => _orderDAL.GetList(customerId);

        public List<OrderDetail> GetOrderDetails(int orderID) => _orderDAL.GetOrderDetails(orderID);

        public bool UpdateStatus(int orderID, int status) => _orderDAL.UpdateStatus(orderID, status);
    }
}
