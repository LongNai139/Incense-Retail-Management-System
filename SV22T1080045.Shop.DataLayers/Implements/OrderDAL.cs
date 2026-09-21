using Dapper;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class OrderDAL : _BaseDAL, IOrderDAL
    {
        private static bool _paymentColumnsEnsured;

        public OrderDAL(string connectionString) : base(connectionString) { }

        public int AddOrder(Order data)
        {
            using var conn = OpenConnection();
            EnsurePaymentColumns(conn);

            string sql = @"
                INSERT INTO Orders
                    (CustomerId, OrderDate, TotalAmount, Status,
                     ShippingName, ShippingPhone, ShippingAddress,
                     CreatedTime, Isdeleted, Note,
                     PaymentMethod, PaymentStatus,
                     VoucherId, VoucherCode, DiscountAmount, FinalAmount)
                VALUES
                    (@CustomerId, @OrderDate, @TotalAmount, @Status,
                     @ShippingName, @ShippingPhone, @ShippingAddress,
                     @CreatedTime, @Isdeleted, @Note,
                     @PaymentMethod, @PaymentStatus,
                     @VoucherId, @VoucherCode, @DiscountAmount, @FinalAmount);
                SELECT CAST(SCOPE_IDENTITY() AS int);";
            return conn.ExecuteScalar<int>(sql, data);
        }

        public void AddOrderDetail(OrderDetail data)
        {
            using var conn = OpenConnection();
            string sql = @"
                INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice)
                VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)";
            conn.Execute(sql, data);
        }

        public Order? GetOrder(int orderID)
        {
            using var conn = OpenConnection();
            EnsurePaymentColumns(conn);

            return conn.QueryFirstOrDefault<Order>(
                "SELECT * FROM Orders WHERE Id = @orderID",
                new { orderID });
        }

        public List<Order> GetList(int customerId)
        {
            using var conn = OpenConnection();
            EnsurePaymentColumns(conn);

            return conn.Query<Order>(
                "SELECT * FROM Orders WHERE CustomerId = @customerId ORDER BY OrderDate DESC",
                new { customerId }).ToList();
        }

        public List<OrderDetail> GetOrderDetails(int orderID)
        {
            using var conn = OpenConnection();
            string sql = @"
                SELECT d.*, p.ProductName, p.ImageUrl AS Photo
                FROM   OrderDetails d
                JOIN   Products p ON d.ProductId = p.Id
                WHERE  d.OrderId = @orderID";
            return conn.Query<OrderDetail>(sql, new { orderID }).ToList();
        }

        public bool UpdateStatus(int orderID, int status)
        {
            using var conn = OpenConnection();
            return conn.Execute(
                "UPDATE Orders SET Status = @status WHERE Id = @orderID",
                new { status, orderID }) > 0;
        }

        public bool UpdatePaymentResult(
            int orderID,
            int paymentStatus,
            string? transactionNo,
            string? bankCode,
            string? responseCode,
            DateTime? paidAt)
        {
            using var conn = OpenConnection();
            EnsurePaymentColumns(conn);

            return conn.Execute(@"
                UPDATE Orders
                SET PaymentStatus = @paymentStatus,
                    PaymentTransactionNo = @transactionNo,
                    PaymentBankCode = @bankCode,
                    PaymentResponseCode = @responseCode,
                    PaidAt = @paidAt
                WHERE Id = @orderID",
                new { orderID, paymentStatus, transactionNo, bankCode, responseCode, paidAt }) > 0;
        }

        private static void EnsurePaymentColumns(System.Data.IDbConnection conn)
        {
            if (_paymentColumnsEnsured)
                return;

            conn.Execute(@"
                IF COL_LENGTH('Orders', 'Note') IS NULL
                    ALTER TABLE Orders ADD Note nvarchar(500) NULL;
                IF COL_LENGTH('Orders', 'PaymentMethod') IS NULL
                    ALTER TABLE Orders ADD PaymentMethod int NOT NULL CONSTRAINT DF_Orders_PaymentMethod DEFAULT 1;
                IF COL_LENGTH('Orders', 'PaymentStatus') IS NULL
                    ALTER TABLE Orders ADD PaymentStatus int NOT NULL CONSTRAINT DF_Orders_PaymentStatus DEFAULT 0;
                IF COL_LENGTH('Orders', 'PaymentTransactionNo') IS NULL
                    ALTER TABLE Orders ADD PaymentTransactionNo nvarchar(100) NULL;
                IF COL_LENGTH('Orders', 'PaymentBankCode') IS NULL
                    ALTER TABLE Orders ADD PaymentBankCode nvarchar(50) NULL;
                IF COL_LENGTH('Orders', 'PaymentResponseCode') IS NULL
                    ALTER TABLE Orders ADD PaymentResponseCode nvarchar(20) NULL;
                IF COL_LENGTH('Orders', 'PaidAt') IS NULL
                    ALTER TABLE Orders ADD PaidAt datetime2 NULL;

                -- Voucher columns
                IF COL_LENGTH('Orders', 'VoucherId') IS NULL
                    ALTER TABLE Orders ADD VoucherId int NULL;
                IF COL_LENGTH('Orders', 'VoucherCode') IS NULL
                    ALTER TABLE Orders ADD VoucherCode nvarchar(50) NULL;
                IF COL_LENGTH('Orders', 'DiscountAmount') IS NULL
                    ALTER TABLE Orders ADD DiscountAmount decimal(18,2) NOT NULL CONSTRAINT DF_Orders_DiscountAmount DEFAULT 0;
                IF COL_LENGTH('Orders', 'FinalAmount') IS NULL
                    ALTER TABLE Orders ADD FinalAmount decimal(18,2) NOT NULL CONSTRAINT DF_Orders_FinalAmount DEFAULT 0;");

            _paymentColumnsEnsured = true;
        }
    }
}