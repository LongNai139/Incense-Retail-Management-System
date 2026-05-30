using Dapper;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models.Management;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class ManagementDAL : _BaseDAL, IManagementDAL
    {
        public ManagementDAL(string connectionString) : base(connectionString) { }

        public bool VoucherCodeExists(string code, int exceptId)
        {
            using var conn = OpenConnection();
            return conn.ExecuteScalar<int>(@"
                SELECT COUNT(1)
                FROM Vouchers
                WHERE IsDeleted = 0 AND Code = @Code AND Id <> @ExceptId",
                new { Code = code.Trim().ToUpperInvariant(), ExceptId = exceptId }) > 0;
        }

        public int AddVoucher(Voucher voucher)
        {
            using var conn = OpenConnection();
            return conn.ExecuteScalar<int>(@"
                INSERT INTO Vouchers
                    (Code, Description, DiscountType, DiscountValue, MaxDiscount,
                     MinOrderAmount, ExpiresAt, MaxUsage, UsedCount, IsActive, CreatedTime, IsDeleted)
                VALUES
                    (@Code, @Description, @DiscountType, @DiscountValue, @MaxDiscount,
                     @MinOrderAmount, @ExpiresAt, @MaxUsage, @UsedCount, @IsActive, @CreatedTime, @IsDeleted);
                SELECT CAST(SCOPE_IDENTITY() AS int);", voucher);
        }

        public Voucher? GetVoucher(int id)
        {
            using var conn = OpenConnection();
            return conn.QueryFirstOrDefault<Voucher>(
                "SELECT * FROM Vouchers WHERE Id = @id AND IsDeleted = 0",
                new { id });
        }

        public bool SetVoucherActive(int id, bool isActive)
        {
            using var conn = OpenConnection();
            return conn.Execute(
                "UPDATE Vouchers SET IsActive = @isActive WHERE Id = @id AND IsDeleted = 0",
                new { id, isActive }) > 0;
        }

        public List<Voucher> ListVouchers(int take)
        {
            using var conn = OpenConnection();
            return conn.Query<Voucher>(@"
                SELECT TOP (@Take) *
                FROM Vouchers
                WHERE IsDeleted = 0
                ORDER BY CreatedTime DESC", new { Take = take }).ToList();
        }

        public List<ManagementOrderData> ListRecentOrders(int take)
        {
            using var conn = OpenConnection();
            var rows = conn.Query<ManagementOrderData>(@"
                SELECT TOP (@Take)
                    o.Id,
                    o.OrderDate,
                    o.TotalAmount,
                    o.Status,
                    COALESCE(NULLIF(c.CustomerName, ''), NULLIF(o.ShippingName, ''), 'Guest') AS CustomerName,
                    COALESCE(NULLIF(c.Phone, ''), NULLIF(o.ShippingPhone, ''), '') AS CustomerPhone,
                    CAST(CASE WHEN c.Id IS NULL OR o.CustomerId <= 0 THEN 1 ELSE 0 END AS bit) AS IsGuest,
                    COALESCE(SUM(d.Quantity), 0) AS ItemCount
                FROM Orders o
                LEFT JOIN Customers c ON o.CustomerId = c.Id
                LEFT JOIN OrderDetails d ON o.Id = d.OrderId
                WHERE o.IsDeleted = 0
                GROUP BY o.Id, o.OrderDate, o.TotalAmount, o.Status,
                    COALESCE(NULLIF(c.CustomerName, ''), NULLIF(o.ShippingName, ''), 'Guest'),
                    COALESCE(NULLIF(c.Phone, ''), NULLIF(o.ShippingPhone, ''), ''),
                    CAST(CASE WHEN c.Id IS NULL OR o.CustomerId <= 0 THEN 1 ELSE 0 END AS bit)
                ORDER BY o.OrderDate DESC", new { Take = take }).ToList();

            return rows;
        }

        public List<CustomerManagementData> ListCustomers()
        {
            using var conn = OpenConnection();
            return conn.Query<CustomerManagementData>(@"
                SELECT
                    c.Id,
                    c.CustomerName,
                    c.Phone,
                    c.Email,
                    c.Address,
                    c.Role,
                    c.CreatedTime,
                    COUNT(o.Id) AS OrderCount,
                    COALESCE(SUM(o.TotalAmount), 0) AS TotalSpent,
                    MAX(o.OrderDate) AS LastOrderDate
                FROM Customers c
                LEFT JOIN Orders o ON c.Id = o.CustomerId AND o.IsDeleted = 0
                WHERE c.IsDeleted = 0
                GROUP BY c.Id, c.CustomerName, c.Phone, c.Email, c.Address, c.Role, c.CreatedTime
                ORDER BY COALESCE(MAX(o.OrderDate), c.CreatedTime) DESC").ToList();
        }

        public List<CustomerPurchaseHistoryData> ListCustomerHistory(int customerId, int take)
        {
            using var conn = OpenConnection();
            var histories = conn.Query<CustomerPurchaseHistoryData>(@"
                SELECT TOP (@Take)
                    o.Id AS OrderId,
                    o.OrderDate,
                    o.TotalAmount,
                    o.Status,
                    COALESCE(SUM(d.Quantity), 0) AS ItemCount
                FROM Orders o
                LEFT JOIN OrderDetails d ON o.Id = d.OrderId
                WHERE o.IsDeleted = 0 AND o.CustomerId = @CustomerId
                GROUP BY o.Id, o.OrderDate, o.TotalAmount, o.Status
                ORDER BY o.OrderDate DESC",
                new { CustomerId = customerId, Take = take }).ToList();

            var orderIds = histories.Select(h => h.OrderId).ToArray();
            if (orderIds.Length == 0)
                return histories;

            var products = conn.Query<(int OrderId, string ProductName)>(@"
                SELECT d.OrderId, p.ProductName
                FROM OrderDetails d
                INNER JOIN Products p ON d.ProductId = p.Id
                WHERE d.OrderId IN @OrderIds",
                new { OrderIds = orderIds }).ToList();

            foreach (var history in histories)
            {
                history.ProductNames = products
                    .Where(p => p.OrderId == history.OrderId)
                    .Select(p => p.ProductName)
                    .Take(3)
                    .ToList();
            }

            return histories;
        }

        public ManagementOrderDetailsData? GetOrderDetails(int orderId)
        {
            using var conn = OpenConnection();
            var order = conn.QueryFirstOrDefault<ManagementOrderDetailHeaderData>(@"
                SELECT
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
                WHERE o.IsDeleted = 0 AND o.Id = @OrderId",
                new { OrderId = orderId });

            if (order == null)
                return null;

            var details = conn.Query<ManagementOrderDetailLineData>(@"
                SELECT
                    p.ProductName,
                    d.Quantity,
                    d.UnitPrice,
                    d.Quantity * d.UnitPrice AS LineTotal
                FROM OrderDetails d
                INNER JOIN Products p ON d.ProductId = p.Id
                WHERE d.OrderId = @OrderId",
                new { OrderId = orderId }).ToList();

            return new ManagementOrderDetailsData
            {
                Order = order,
                Details = details
            };
        }
    }
}
