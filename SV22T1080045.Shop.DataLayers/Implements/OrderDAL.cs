using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.DomainModels.Warehouse;

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

            if (status == 4 && order.Status != 4 && order.PaymentMethod == 1 && order.CustomerId > 0 &&
                !_context.CustomerDebts.Any(d => d.OrderId == order.Id && !d.IsDeleted))
            {
                var amount = order.FinalAmount > 0
                    ? order.FinalAmount
                    : Math.Max(0, order.TotalAmount - order.DiscountAmount);
                var now = DateTime.Now;
                var debt = new CustomerDebt
                {
                    CustomerId = order.CustomerId,
                    OrderId = order.Id,
                    OrderCode = $"TH{order.Id:D6}",
                    OrderAmount = amount,
                    DebtAmount = amount,
                    PaidAmount = 0,
                    DueDate = DateTime.Today.AddDays(30),
                    Status = "Pending",
                    Notes = "Phát sinh khi giao thành công đơn COD",
                    CreatedTime = now,
                    IsDeleted = false
                };
                debt.Transactions.Add(new CustomerDebtTransaction
                {
                    OrderId = order.Id,
                    TransactionType = "DEBIT",
                    Amount = amount,
                    BalanceAfter = amount,
                    Reference = $"ORDER-{order.Id}",
                    Description = "Phát sinh từ đơn hàng COD đã giao thành công",
                    TransactionDate = now,
                    CreatedTime = now,
                    IsDeleted = false
                });
                _context.CustomerDebts.Add(debt);
            }

            order.Status = status;
            try
            {
                return _context.SaveChanges() > 0;
            }
            catch (DbUpdateException exception) when (IsDebtUniqueConstraintViolation(exception))
            {
                return false;
            }
        }

        private static bool IsDebtUniqueConstraintViolation(DbUpdateException exception)
        {
            return exception.GetBaseException() is SqlException sqlException &&
                   sqlException.Number is 2601 or 2627;
        }

        public bool UpdatePaymentResult(
            int orderID,
            int paymentStatus,
            string? transactionNo,
            string? bankCode,
            string? responseCode,
            DateTime? paidAt)
        {
            if (paymentStatus is not (1 or 2))
                return false;

            return _context.Orders
                .Where(o => o.Id == orderID && (paymentStatus == 1 || o.PaymentStatus != 1))
                .ExecuteUpdate(setters => setters
                    .SetProperty(o => o.PaymentStatus, paymentStatus)
                    .SetProperty(o => o.PaymentTransactionNo, transactionNo)
                    .SetProperty(o => o.PaymentBankCode, bankCode)
                    .SetProperty(o => o.PaymentResponseCode, responseCode)
                    .SetProperty(o => o.PaidAt, paidAt)) > 0;
        }
    }
}
