using Dapper;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models.Reports;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class RevenueReportDAL : _BaseDAL, IRevenueReportDAL
    {
        public RevenueReportDAL(string connectionString) : base(connectionString) { }

        public RevenueReportData GetRevenueReport(DateTime start, DateTime end, DateTime previousStart, DateTime previousEnd)
        {
            using var conn = OpenConnection();
            var args = new
            {
                Start = start,
                End = end,
                PreviousStart = previousStart,
                PreviousEnd = previousEnd
            };

            var summary = conn.QuerySingle<RevenueReportSummary>(@"
                SELECT
                    COALESCE(SUM(o.TotalAmount), 0) AS Revenue,
                    COALESCE(SUM(CASE WHEN o.Status = 4 THEN 1 ELSE 0 END), 0) AS CompletedOrderCount,
                    COALESCE(SUM(CASE WHEN o.CustomerId <= 0 THEN 1 ELSE 0 END), 0) AS GuestOrderCount,
                    COUNT(1) AS OrderCount,
                    (
                        SELECT COUNT(1)
                        FROM Customers c
                        WHERE c.IsDeleted = 0
                          AND ISNULL(c.Role, '') <> 'Admin'
                          AND c.CreatedTime >= @Start
                          AND c.CreatedTime < @End
                    ) AS NewCustomerCount,
                    (
                        SELECT COALESCE(SUM(po.TotalAmount), 0)
                        FROM Orders po
                        WHERE po.IsDeleted = 0
                          AND po.OrderDate >= @PreviousStart
                          AND po.OrderDate < @PreviousEnd
                    ) AS PreviousRevenue
                FROM Orders o
                WHERE o.IsDeleted = 0
                  AND o.OrderDate >= @Start
                  AND o.OrderDate < @End", args);

            var productRows = conn.Query<ProductRevenueReportRow>(@"
                SELECT TOP 20
                    d.ProductId,
                    p.ProductName,
                    SUM(d.Quantity) AS Quantity,
                    SUM(d.Quantity * d.UnitPrice) AS Revenue,
                    COUNT(DISTINCT d.OrderId) AS OrderCount
                FROM OrderDetails d
                INNER JOIN Orders o ON d.OrderId = o.Id
                INNER JOIN Products p ON d.ProductId = p.Id
                WHERE o.IsDeleted = 0
                  AND o.OrderDate >= @Start
                  AND o.OrderDate < @End
                GROUP BY d.ProductId, p.ProductName
                ORDER BY Revenue DESC", args).ToList();

            var customerRows = conn.Query<CustomerRevenueReportRow>(@"
                SELECT TOP 20
                    COALESCE(c.Id, 0) AS CustomerId,
                    COALESCE(NULLIF(c.CustomerName, ''), NULLIF(o.ShippingName, ''), 'Guest') AS CustomerName,
                    COALESCE(NULLIF(c.Phone, ''), NULLIF(o.ShippingPhone, ''), '') AS Phone,
                    COUNT(o.Id) AS OrderCount,
                    SUM(o.TotalAmount) AS Revenue
                FROM Orders o
                LEFT JOIN Customers c ON o.CustomerId = c.Id
                WHERE o.IsDeleted = 0
                  AND o.OrderDate >= @Start
                  AND o.OrderDate < @End
                GROUP BY
                    COALESCE(c.Id, 0),
                    COALESCE(NULLIF(c.CustomerName, ''), NULLIF(o.ShippingName, ''), 'Guest'),
                    COALESCE(NULLIF(c.Phone, ''), NULLIF(o.ShippingPhone, ''), '')
                ORDER BY Revenue DESC", args).ToList();

            var orderRows = conn.Query<OrderRevenueReportRow>(@"
                SELECT
                    o.Id AS OrderId,
                    o.OrderDate,
                    COALESCE(NULLIF(c.CustomerName, ''), NULLIF(o.ShippingName, ''), 'Guest') AS CustomerName,
                    COALESCE(NULLIF(c.Phone, ''), NULLIF(o.ShippingPhone, ''), '') AS CustomerPhone,
                    o.Status,
                    COALESCE(SUM(d.Quantity), 0) AS ItemCount,
                    o.TotalAmount
                FROM Orders o
                LEFT JOIN Customers c ON o.CustomerId = c.Id
                LEFT JOIN OrderDetails d ON o.Id = d.OrderId
                WHERE o.IsDeleted = 0
                  AND o.OrderDate >= @Start
                  AND o.OrderDate < @End
                GROUP BY
                    o.Id,
                    o.OrderDate,
                    COALESCE(NULLIF(c.CustomerName, ''), NULLIF(o.ShippingName, ''), 'Guest'),
                    COALESCE(NULLIF(c.Phone, ''), NULLIF(o.ShippingPhone, ''), ''),
                    o.Status,
                    o.TotalAmount
                ORDER BY o.OrderDate DESC", args).ToList();

            return new RevenueReportData
            {
                Summary = summary,
                ProductRows = productRows,
                CustomerRows = customerRows,
                OrderRows = orderRows
            };
        }
    }
}
