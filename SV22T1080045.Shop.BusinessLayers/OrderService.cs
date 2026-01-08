using SV22T1080045.Shop.DomainModels;

using System;
using System.Collections.Generic;
using System.Linq;
using SV22T1080045.Shop.DataLayers;

namespace SV22T1080045.Shop.BusinessLayers
{
    public class OrderService
    {
        private readonly OrderDAL _orderDAL;

        public OrderService(OrderDAL orderDAL)
        {
            _orderDAL = orderDAL;
        }

        /// <summary>
        /// Khởi tạo đơn hàng (Xử lý logic tính tiền và lưu xuống DB)
        /// </summary>
        public int InitOrder(int customerID, string deliveryAddress, string deliveryPhone, List<CartItem> cart)
        {
            try
            {
                if (cart == null || cart.Count == 0) return 0;

                // Tạo đối tượng đơn hàng 
                var order = new Orders()
                {
                    CustomerID = customerID,
                    OrderTime = DateTime.Now,
                    DeliveryAddress = deliveryAddress,
                    DeliveryPhone = deliveryPhone,
                    Status = "Chờ xử lý",
                    TotalAmount = cart.Sum(item => item.TotalPrice)
                };

                // Gọi DAL để lưu Orders và lấy về OrderID vừa sinh ra
                int orderID = _orderDAL.AddOrder(order);

                if (orderID > 0)
                {
                    // Lưu chi tiết đơn hàng (Details)
                    foreach (var item in cart)
                    {
                        var detail = new OrderDetails()
                        {
                            OrderID = orderID,
                            ProductID = item.ProductID,
                            Quantity = item.Quantity,
                            SalePrice = item.Price
                        };
                        _orderDAL.AddOrderDetail(detail);
                    }
                    return orderID;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Lấy danh sách đơn hàng của một khách hàng (Dùng cho trang History)
        /// </summary>
        public List<Orders> GetCustomerOrders(int customerID)
        {
            return _orderDAL.GetList(customerID);
        }

        /// <summary>
        /// Lấy thông tin một đơn hàng theo ID (Dùng cho trang Details)
        /// </summary>
        public Orders GetOrder(int orderID)
        {
            return _orderDAL.GetOrder(orderID);
        }

        /// <summary>
        /// Lấy danh sách chi tiết sản phẩm của đơn hàng (Dùng cho trang Details)
        /// </summary>
        public List<OrderDetails> GetOrderDetails(int orderID)
        {
            return _orderDAL.GetOrderDetails(orderID);
        }
    }
}