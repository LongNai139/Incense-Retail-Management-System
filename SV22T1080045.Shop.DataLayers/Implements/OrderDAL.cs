using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class OrderDAL : IOrderDAL
    {
        private readonly ShopDbContext _context;

        public OrderDAL(ShopDbContext context)
        {
            _context = context;
        }

        public int AddOrder(Order data)
        {
            _context.Orders.Add(data);
            _context.SaveChanges();
            return data.Id;
        }

        public void AddOrderDetail(OrderDetail data)
        {
            _context.OrderDetails.Add(data);
            _context.SaveChanges();
        }

        public Order? GetOrder(int orderID)
        {
            return _context.Orders
                .FirstOrDefault(o => o.Id == orderID);
        }

        public List<Order> GetList(int customerId)
        {
            return _context.Orders
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public List<OrderDetail> GetOrderDetails(int orderID)
        {
            return _context.OrderDetails
                .Include(d => d.Product)
                .Where(d => d.OrderId == orderID)
                .ToList();
        }

        public bool UpdateStatus(int orderID, int status)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == orderID);
            if (order == null)
                return false;

            order.Status = status;
            return _context.SaveChanges() > 0;
        }

        public bool UpdatePaymentResult(
            int orderID,
            int paymentStatus,
            string? transactionNo,
            string? bankCode,
            string? responseCode,
            DateTime? paidAt)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == orderID);
            if (order == null)
                return false;

            order.PaymentStatus = paymentStatus;
            order.PaymentTransactionNo = transactionNo;
            order.PaymentBankCode = bankCode;
            order.PaymentResponseCode = responseCode;
            order.PaidAt = paidAt;

            return _context.SaveChanges() > 0;
        }
    }
}
