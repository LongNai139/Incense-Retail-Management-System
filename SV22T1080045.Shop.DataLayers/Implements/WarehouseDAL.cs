using Dapper;
using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.DomainModels.Warehouse;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class WarehouseDAL : IWarehouseDAL
    {
        private readonly ShopDbContext _context;

        public WarehouseDAL(ShopDbContext context)
        {
            _context = context;
        }

        public List<StockTransaction> ListStockTransactions(int productId, DateTime? fromDate, DateTime? toDate)
        {
            using var conn = _context.GetConnection();
            var sql = @"
                SELECT st.*, p.ProductName
                FROM StockTransactions st
                LEFT JOIN Products p ON st.ProductId = p.Id
                WHERE st.IsDeleted = 0";
            
            if (productId > 0)
                sql += " AND st.ProductId = @ProductId";
            
            if (fromDate.HasValue)
                sql += " AND st.TransactionDate >= @FromDate";
            
            if (toDate.HasValue)
                sql += " AND st.TransactionDate <= @ToDate";
            
            sql += " ORDER BY st.TransactionDate DESC";
            
            return conn.Query<StockTransaction>(sql, new { ProductId = productId, FromDate = fromDate, ToDate = toDate }).ToList();
        }

        public StockTransaction? GetStockTransaction(int id)
        {
            using var conn = _context.GetConnection();
            return conn.QueryFirstOrDefault<StockTransaction>(
                "SELECT * FROM StockTransactions WHERE Id = @Id AND IsDeleted = 0",
                new { Id = id });
        }

        public int AddStockTransaction(StockTransaction transaction)
        {
            using var conn = _context.GetConnection();
            return conn.ExecuteScalar<int>(@"
                INSERT INTO StockTransactions
                    (ProductId, Quantity, TransactionType, UnitPrice, ReferenceNumber, Reason,
                     TransactionDate, CreatedTime, CreatedBy, IsDeleted)
                VALUES
                    (@ProductId, @Quantity, @TransactionType, @UnitPrice, @ReferenceNumber, @Reason,
                     @TransactionDate, @CreatedTime, @CreatedBy, @IsDeleted);
                SELECT CAST(SCOPE_IDENTITY() AS int);", transaction);
        }

        public bool UpdateStockTransaction(StockTransaction transaction)
        {
            using var conn = _context.GetConnection();
            return conn.Execute(@"
                UPDATE StockTransactions
                SET Quantity = @Quantity, TransactionType = @TransactionType, UnitPrice = @UnitPrice,
                    ReferenceNumber = @ReferenceNumber, Reason = @Reason, TransactionDate = @TransactionDate,
                    UpdatedTime = GETDATE()
                WHERE Id = @Id AND IsDeleted = 0", transaction) > 0;
        }

        public bool DeleteStockTransaction(int id)
        {
            using var conn = _context.GetConnection();
            return conn.Execute(
                "UPDATE StockTransactions SET IsDeleted = 1 WHERE Id = @Id",
                new { Id = id }) > 0;
        }

        public List<StockInventory> ListStockInventories()
        {
            using var conn = _context.GetConnection();
            return conn.Query<StockInventory>(
                @"SELECT pi.ProductId, p.ProductName, pi.Quantity as CurrentQuantity, pi.LowStockThreshold, pi.UpdatedTime as LastUpdated, GETDATE() as CreatedTime, 0 as AverageCost, 1000 as MaxStockThreshold
                FROM ProductInventories pi
                LEFT JOIN Products p ON pi.ProductId = p.Id
                ORDER BY pi.Quantity ASC").ToList();
        }

        public StockInventory? GetStockInventory(int productId)
        {
            using var conn = _context.GetConnection();
            return conn.QueryFirstOrDefault<StockInventory>(
                @"SELECT pi.ProductId, p.ProductName, pi.Quantity as CurrentQuantity, pi.LowStockThreshold, pi.UpdatedTime as LastUpdated, GETDATE() as CreatedTime, 0 as AverageCost, 1000 as MaxStockThreshold
                FROM ProductInventories pi
                LEFT JOIN Products p ON pi.ProductId = p.Id
                WHERE pi.ProductId = @ProductId",
                new { ProductId = productId });
        }

        public bool UpdateStockInventory(int productId, int quantityChange, decimal averageCost)
        {
            using var conn = _context.GetConnection();
            var existing = GetStockInventory(productId);

            if (existing == null)
            {
                // Create new inventory record
                return conn.Execute(@"
                    INSERT INTO ProductInventories
                        (ProductId, Quantity, LowStockThreshold, UpdatedTime)
                    VALUES
                        (@ProductId, @Quantity, 5, GETDATE())",
                    new { ProductId = productId, Quantity = quantityChange }) > 0;
            }
            else
            {
                // Update existing inventory
                return conn.Execute(@"
                    UPDATE ProductInventories
                    SET Quantity = Quantity + @QuantityChange,
                        UpdatedTime = GETDATE()
                    WHERE ProductId = @ProductId",
                    new { ProductId = productId, QuantityChange = quantityChange }) > 0;
            }
        }

        public List<CustomerDebt> ListCustomerDebts(int? customerId)
        {
            using var conn = _context.GetConnection();
            var sql = @"
                SELECT cd.*, c.CustomerName
                FROM CustomerDebts cd
                LEFT JOIN Customers c ON cd.CustomerId = c.Id
                WHERE cd.IsDeleted = 0";

            if (customerId.HasValue)
                sql += " AND cd.CustomerId = @CustomerId";

            sql += " ORDER BY cd.DueDate ASC";

            return conn.Query<CustomerDebt>(sql, new { CustomerId = customerId }).ToList();
        }

        public CustomerDebt? GetCustomerDebt(int id)
        {
            using var conn = _context.GetConnection();
            return conn.QueryFirstOrDefault<CustomerDebt>(
                "SELECT * FROM CustomerDebts WHERE Id = @Id AND IsDeleted = 0",
                new { Id = id });
        }

        public int AddCustomerDebt(CustomerDebt debt)
        {
            using var conn = _context.GetConnection();
            return conn.ExecuteScalar<int>(@"
                INSERT INTO CustomerDebts
                    (CustomerId, OrderId, OrderCode, OrderAmount, DebtAmount, PaidAmount, PaymentReference, DueDate, PaidDate,
                     Status, Notes, CreatedTime, UpdatedTime, IsDeleted)
                VALUES
                    (@CustomerId, @OrderId, @OrderCode, @OrderAmount, @DebtAmount, @PaidAmount, @PaymentReference, @DueDate, @PaidDate,
                     @Status, @Notes, @CreatedTime, @UpdatedTime, @IsDeleted);
                SELECT CAST(SCOPE_IDENTITY() AS int);", debt);
        }

        public bool UpdateCustomerDebt(CustomerDebt debt)
        {
            using var conn = _context.GetConnection();
            return conn.Execute(@"
                UPDATE CustomerDebts
                SET PaidAmount = @PaidAmount, PaidDate = @PaidDate, Status = @Status,
                    Notes = @Notes, UpdatedTime = GETDATE()
                WHERE Id = @Id AND IsDeleted = 0", debt) > 0;
        }

        public bool DeleteCustomerDebt(int id)
        {
            using var conn = _context.GetConnection();
            return conn.Execute(
                "UPDATE CustomerDebts SET IsDeleted = 1 WHERE Id = @Id",
                new { Id = id }) > 0;
        }
    }
}