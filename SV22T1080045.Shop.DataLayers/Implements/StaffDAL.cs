using Dapper;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models.Staff;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class StaffDAL : _BaseDAL, IStaffDAL
    {
        public StaffDAL(string connectionString) : base(connectionString) { }

        public StaffDashboardData GetDashboardData()
        {
            using var conn = OpenConnection();
            var orders = conn.Query<StaffOrderData>(@"
                SELECT TOP 120
                    o.Id,
                    o.OrderDate,
                    o.TotalAmount,
                    o.Status,
                    COALESCE(NULLIF(c.CustomerName, ''), NULLIF(o.ShippingName, ''), 'Guest') AS CustomerName,
                    COALESCE(NULLIF(c.Phone, ''), NULLIF(o.ShippingPhone, ''), '') AS CustomerPhone,
                    o.ShippingAddress,
                    CAST(CASE WHEN c.Id IS NULL OR o.CustomerId <= 0 THEN 1 ELSE 0 END AS bit) AS IsGuest
                FROM Orders o
                LEFT JOIN Customers c ON o.CustomerId = c.Id
                WHERE o.IsDeleted = 0
                ORDER BY o.OrderDate DESC").ToList();

            var orderIds = orders.Select(o => o.Id).ToArray();
            var items = orderIds.Length == 0
                ? new List<StaffOrderItemData>()
                : conn.Query<StaffOrderItemData>(@"
                    SELECT
                        d.OrderId,
                        p.ProductName,
                        d.Quantity,
                        d.UnitPrice
                    FROM OrderDetails d
                    INNER JOIN Products p ON d.ProductId = p.Id
                    WHERE d.OrderId IN @OrderIds", new { OrderIds = orderIds }).ToList();

            foreach (var order in orders)
            {
                order.Items = items.Where(i => i.OrderId == order.Id).ToList();
                order.ItemCount = order.Items.Sum(i => i.Quantity);
            }

            var products = conn.Query<StaffProductData>(@"
                SELECT
                    p.Id,
                    p.ProductName,
                    COALESCE(c.CategoryName, 'Chua phan loai') AS CategoryName,
                    COALESCE(u.UnitName, '') AS UnitName,
                    COALESCE(p.Quantity, 0) AS Quantity,
                    COALESCE(p.SoldCount, 0) AS SoldCount,
                    p.PriceAfterDiscount,
                    p.Origin
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                LEFT JOIN Units u ON p.UnitId = u.Id
                WHERE p.IsDeleted = 0").ToList();

            var customers = conn.Query<StaffCustomerData>(@"
                SELECT TOP 20
                    c.Id,
                    c.CustomerName,
                    c.Phone,
                    c.Email,
                    COUNT(o.Id) AS OrderCount,
                    MAX(o.OrderDate) AS LastOrderDate
                FROM Customers c
                LEFT JOIN Orders o ON c.Id = o.CustomerId AND o.IsDeleted = 0
                WHERE c.IsDeleted = 0 AND c.Role <> 'Admin' AND c.Role <> 'Staff'
                GROUP BY c.Id, c.CustomerName, c.Phone, c.Email
                ORDER BY MAX(o.OrderDate) DESC").ToList();

            var customerIds = customers.Select(c => c.Id).ToArray();
            var recentProducts = customerIds.Length == 0
                ? new List<(int CustomerId, string ProductName)>()
                : conn.Query<(int CustomerId, string ProductName)>(@"
                    SELECT o.CustomerId, p.ProductName
                    FROM Orders o
                    INNER JOIN OrderDetails d ON o.Id = d.OrderId
                    INNER JOIN Products p ON d.ProductId = p.Id
                    WHERE o.IsDeleted = 0 AND o.CustomerId IN @CustomerIds
                    ORDER BY o.OrderDate DESC", new { CustomerIds = customerIds }).ToList();

            foreach (var customer in customers)
            {
                customer.RecentProducts = recentProducts
                    .Where(p => p.CustomerId == customer.Id)
                    .Select(p => p.ProductName)
                    .Distinct()
                    .Take(3)
                    .ToList();
            }

            return new StaffDashboardData
            {
                Orders = orders,
                Products = products,
                Customers = customers
            };
        }

        public bool UpdateOrderStatus(int orderId, int status)
        {
            using var conn = OpenConnection();
            return conn.Execute(
                @"UPDATE Orders
                  SET Status = @Status
                  WHERE Id = @OrderId
                    AND IsDeleted = 0
                    AND OrderDate >= CAST(GETDATE() AS date)
                    AND OrderDate < DATEADD(day, 1, CAST(GETDATE() AS date))",
                new { OrderId = orderId, Status = status }) > 0;
        }
    }
}
