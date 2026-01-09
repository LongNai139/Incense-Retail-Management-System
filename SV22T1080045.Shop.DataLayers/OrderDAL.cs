//using Dapper;
//using SV22T1080045.Shop.DomainModels;
//using System.Data;
//using System.Collections.Generic;
//using System.Linq;

//namespace SV22T1080045.Shop.DataLayers
//{
//    public class OrderDAL : BaseDAL
//    {
//        public OrderDAL(string connectionString) : base(connectionString) { }

//        // Tạo đơn hàng
//        public int AddOrder(Orders data)
//        {
//            using var conn = OpenConnection();

//            string sql = @"INSERT INTO Orders(CustomerID, OrderTime, DeliveryAddress, DeliveryPhone, Status, TotalAmount)
//                           VALUES(@CustomerID, GETDATE(), @DeliveryAddress, @DeliveryPhone, @Status, @TotalAmount);
                           
//                           SELECT SCOPE_IDENTITY();";

//            if (string.IsNullOrEmpty(data.Status)) data.Status = "Chờ xử lý";

//            return conn.ExecuteScalar<int>(sql, data);
//        }

//        // Thêm chi tiết
//        public void AddOrderDetail(OrderDetails data)
//        {
//            using var conn = OpenConnection();
//            string sql = @"INSERT INTO OrderDetails(OrderID, ProductID, Quantity, SalePrice)
//                           VALUES(@OrderID, @ProductID, @Quantity, @SalePrice)";
//            conn.Execute(sql, data);
//        }

//        // Lấy danh sách đơn hàng của 1 khách hàng
//        public List<Orders> GetList(int customerID)
//        {
//            using var conn = OpenConnection();
//            // Sắp xếp ngày mới nhất lên đầu
//            var sql = @"SELECT * FROM Orders WHERE CustomerID = @CustomerID ORDER BY OrderTime DESC";
//            return conn.Query<Orders>(sql, new { CustomerID = customerID }).ToList();
//        }

//        // Lấy thông tin 1 đơn hàng theo ID
//        public Orders? GetOrder(int orderID)
//        {
//            using var conn = OpenConnection();
//            var sql = @"SELECT * FROM Orders WHERE OrderID = @OrderID";
//            return conn.QueryFirstOrDefault<Orders>(sql, new { OrderID = orderID });
//        }

//        // Lấy danh sách chi tiết sản phẩm trong đơn hàng
//        public List<OrderDetails> GetOrderDetails(int orderID)
//        {
//            using var conn = OpenConnection();
//            var sql = @"SELECT d.*, p.ProductName, p.Photo
//                        FROM OrderDetails d
//                        JOIN Products p ON d.ProductID = p.ProductID
//                        WHERE d.OrderID = @OrderID";
//            return conn.Query<OrderDetails>(sql, new { OrderID = orderID }).ToList();
//        }
//    }
//}