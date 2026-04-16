using Dapper;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers
{
    public class OrderDAL : _BaseDAL
    {
        public OrderDAL(string connectionString) : base(connectionString) { }

        // ── TẠO ĐƠN HÀNG ──────────────────────────────────────────────────────
        public int AddOrder(Order data)
        {
            using var conn = OpenConnection();
            string sql = @"
                INSERT INTO Orders
                    (CustomerId, OrderDate, TotalAmount, Status,
                     ShippingName, ShippingPhone, ShippingAddress)
                VALUES
                    (@CustomerId, GETDATE(), @TotalAmount, @Status,
                     @ShippingName, @ShippingPhone, @ShippingAddress);
                SELECT SCOPE_IDENTITY();";
            return conn.ExecuteScalar<int>(sql, data);
        }

        // ── THÊM CHI TIẾT ĐƠN ────────────────────────────────────────────────
        /// <summary>
        /// Dùng tên cột SQL khớp với property của entity OrderDetail:
        /// OrderId (không phải OrderID), ProductId (không phải ProductID)
        /// </summary>
        public void AddOrderDetail(OrderDetail data)
        {
            using var conn = OpenConnection();
            string sql = @"
                INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice)
                VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)";
            conn.Execute(sql, data);
        }

        // ── LẤY 1 ĐƠN HÀNG ───────────────────────────────────────────────────
        public Order? GetOrder(int orderID)
        {
            using var conn = OpenConnection();
            return conn.QueryFirstOrDefault<Order>(
                "SELECT * FROM Orders WHERE Id = @orderID",
                new { orderID });
        }

        // ── LẤY DANH SÁCH ĐƠN CỦA 1 KHÁCH ──────────────────────────────────
        public List<Order> GetList(int customerId)
        {
            using var conn = OpenConnection();
            return conn.Query<Order>(
                "SELECT * FROM Orders WHERE CustomerId = @customerId ORDER BY OrderDate DESC",
                new { customerId }).ToList();
        }

        // ── LẤY CHI TIẾT ĐƠN ────────────────────────────────────────────────
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

        // ── CẬP NHẬT TRẠNG THÁI ─────────────────────────────────────────────
        public bool UpdateStatus(int orderID, int status)
        {
            using var conn = OpenConnection();
            return conn.Execute(
                "UPDATE Orders SET Status = @status WHERE Id = @orderID",
                new { status, orderID }) > 0;
        }
    }
}