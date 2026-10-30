using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models.Warehouse;
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

        public List<StockTransaction> ListStockTransactions(int productId, DateTime? fromDate, DateTime? toDate, string? transactionType = null)
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
                sql += " AND st.TransactionDate < DATEADD(day, 1, CAST(@ToDate AS date))";

            if (!string.IsNullOrWhiteSpace(transactionType))
                sql += " AND st.TransactionType = @TransactionType";
            
            sql += " ORDER BY st.TransactionDate DESC";
            
            return conn.Query<StockTransaction>(sql, new { ProductId = productId, FromDate = fromDate, ToDate = toDate, TransactionType = transactionType?.Trim() }).ToList();
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
            if (conn.State != ConnectionState.Open)
                conn.Open();
            using var dbTransaction = conn.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                var quantityChange = GetStockQuantityDelta(transaction);
                UpdateInventoryInTransaction(conn, dbTransaction, transaction.ProductId, quantityChange);
                var transactionId = conn.ExecuteScalar<int>(@"
                INSERT INTO StockTransactions
                    (ProductId, Quantity, TransactionType, UnitPrice, ReferenceNumber, Reason,
                     TransactionDate, CreatedTime, CreatedBy, IsDeleted)
                VALUES
                    (@ProductId, @Quantity, @TransactionType, @UnitPrice, @ReferenceNumber, @Reason,
                     @TransactionDate, @CreatedTime, @CreatedBy, @IsDeleted);
                SELECT CAST(SCOPE_IDENTITY() AS int);", transaction, dbTransaction);
                dbTransaction.Commit();
                return transactionId;
            }
            catch
            {
                dbTransaction.Rollback();
                return 0;
            }
        }

        public List<ReimportOrderOption> ListOrdersForReimport()
        {
            using var conn = _context.GetConnection();
            return conn.Query<ReimportOrderOption>(@"
                SELECT o.Id AS OrderId, o.OrderDate, COALESCE(c.CustomerName, o.ShippingName, N'Khách hàng') AS CustomerName
                FROM Orders o
                LEFT JOIN Customers c ON c.Id = o.CustomerId
                WHERE o.Status = 4 AND o.IsDeleted = 0
                  AND EXISTS (
                      SELECT 1 FROM OrderDetails od
                      LEFT JOIN (
                          SELECT RelatedOrderDetailId, SUM(CASE WHEN TransactionType = N'Reimport' THEN Quantity ELSE 0 END) AS ReturnedQuantity
                          FROM StockTransactions
                          WHERE IsDeleted = 0
                            AND RelatedOrderDetailId IS NOT NULL
                          GROUP BY RelatedOrderDetailId
                      ) ret ON ret.RelatedOrderDetailId = od.Id
                      WHERE od.OrderId = o.Id AND od.Quantity > COALESCE(ret.ReturnedQuantity, 0))
                ORDER BY o.OrderDate DESC, o.Id DESC").ToList();
        }

        public List<ReimportOrderLine> ListReimportOrderLines(int orderId)
        {
            using var conn = _context.GetConnection();
            return conn.Query<ReimportOrderLine>(@"
                SELECT od.Id AS OrderDetailId, od.OrderId, od.ProductId, p.ProductName,
                       od.Quantity AS ExportedQuantity,
                       COALESCE(ret.ReturnedQuantity, 0) AS ReturnedQuantity,
                       od.UnitPrice, COALESCE(p.ImportPrice, 0) AS ImportPrice
                FROM OrderDetails od
                INNER JOIN Orders o ON o.Id = od.OrderId
                INNER JOIN Products p ON p.Id = od.ProductId
                LEFT JOIN (
                    SELECT RelatedOrderDetailId, SUM(CASE WHEN TransactionType = N'Reimport' THEN Quantity ELSE 0 END) AS ReturnedQuantity
                    FROM StockTransactions
                    WHERE IsDeleted = 0
                      AND RelatedOrderDetailId IS NOT NULL
                    GROUP BY RelatedOrderDetailId
                ) ret ON ret.RelatedOrderDetailId = od.Id
                WHERE od.OrderId = @OrderId AND o.Status = 4 AND o.IsDeleted = 0
                  AND od.Quantity > COALESCE(ret.ReturnedQuantity, 0)
                ORDER BY od.Id", new { OrderId = orderId }).ToList();
        }

        public bool AddReimportTransaction(StockTransaction transaction, int orderId, int orderDetailId, out string error)
        {
            error = "";
            if (transaction.Quantity <= 0)
            {
                error = "Số lượng tái nhập phải lớn hơn 0.";
                return false;
            }

            using var conn = _context.GetConnection();
            if (conn.State != ConnectionState.Open)
                conn.Open();
            using var dbTransaction = conn.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                var source = conn.QueryFirstOrDefault<ReimportOrderSource>(@"
                    SELECT od.ProductId, od.Quantity, o.Status, COALESCE(p.ImportPrice, 0) AS ImportPrice
                    FROM OrderDetails od WITH (UPDLOCK, HOLDLOCK)
                    INNER JOIN Orders o WITH (UPDLOCK, HOLDLOCK) ON o.Id = od.OrderId
                    INNER JOIN Products p ON p.Id = od.ProductId
                    WHERE od.Id = @OrderDetailId AND od.OrderId = @OrderId
                      AND o.IsDeleted = 0 AND od.Quantity > 0",
                    new { OrderId = orderId, OrderDetailId = orderDetailId }, dbTransaction);

                if (source == null || source.Status != 4)
                {
                    error = "Chỉ có thể tái nhập hàng từ đơn đã hoàn thành.";
                    dbTransaction.Rollback();
                    return false;
                }

                var alreadyReturned = conn.ExecuteScalar<int?>(@"
                    SELECT SUM(Quantity) FROM StockTransactions WITH (UPDLOCK, HOLDLOCK)
                    WHERE IsDeleted = 0 AND TransactionType = N'Reimport' AND RelatedOrderDetailId = @OrderDetailId",
                    new { OrderDetailId = orderDetailId }, dbTransaction) ?? 0;
                var remaining = source.Quantity - alreadyReturned;
                if (remaining <= 0 || transaction.Quantity > remaining)
                {
                    error = $"Số lượng có thể tái nhập còn {Math.Max(0, remaining)} sản phẩm.";
                    dbTransaction.Rollback();
                    return false;
                }

                transaction.ProductId = source.ProductId;
                transaction.TransactionType = "Reimport";
                transaction.UnitPrice = source.ImportPrice;
                transaction.RelatedOrderDetailId = orderDetailId;
                transaction.ReferenceNumber = $"DH-{orderId}";
                transaction.CreatedTime = DateTime.Now;
                transaction.TransactionDate = transaction.TransactionDate == default ? DateTime.Now : transaction.TransactionDate;
                transaction.IsDeleted = false;

                UpdateInventoryInTransaction(conn, dbTransaction, transaction.ProductId, transaction.Quantity);

                conn.Execute(@"
                    INSERT INTO StockTransactions
                        (ProductId, Quantity, TransactionType, UnitPrice, ReferenceNumber, Reason, RelatedOrderDetailId,
                         TransactionDate, CreatedTime, CreatedBy, IsDeleted)
                    VALUES
                        (@ProductId, @Quantity, @TransactionType, @UnitPrice, @ReferenceNumber, @Reason, @RelatedOrderDetailId,
                         @TransactionDate, @CreatedTime, @CreatedBy, @IsDeleted)", transaction, dbTransaction);
                dbTransaction.Commit();
                return true;
            }
            catch
            {
                dbTransaction.Rollback();
                error = "Không thể lưu phiếu tái nhập. Vui lòng thử lại.";
                return false;
            }
        }

        public bool UpdateStockTransaction(StockTransaction transaction)
        {
            using var conn = _context.GetConnection();
            if (conn.State != ConnectionState.Open)
                conn.Open();
            using var dbTransaction = conn.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                var existing = conn.QueryFirstOrDefault<StockTransaction>(
                    "SELECT * FROM StockTransactions WITH (UPDLOCK, HOLDLOCK) WHERE Id = @Id AND IsDeleted = 0",
                    new { transaction.Id }, dbTransaction);
                if (existing == null)
                {
                    dbTransaction.Rollback();
                    return false;
                }

                var oldDelta = GetStockQuantityDelta(existing);
                var newDelta = GetStockQuantityDelta(transaction);
                if (existing.ProductId != transaction.ProductId)
                {
                    conn.Execute(@"UPDATE ProductInventories SET Quantity = Quantity - @QuantityChange, UpdatedTime = GETDATE()
                                   WHERE ProductId = @ProductId",
                        new { existing.ProductId, QuantityChange = oldDelta }, dbTransaction);
                    UpdateInventoryInTransaction(conn, dbTransaction, transaction.ProductId, newDelta);
                }
                else
                {
                    UpdateInventoryInTransaction(conn, dbTransaction, transaction.ProductId, newDelta - oldDelta);
                }
                var affected = conn.Execute(@"
                UPDATE StockTransactions
                SET Quantity = @Quantity, TransactionType = @TransactionType, UnitPrice = @UnitPrice,
                    ReferenceNumber = @ReferenceNumber, Reason = @Reason, RelatedOrderDetailId = @RelatedOrderDetailId,
                    TransactionDate = @TransactionDate,
                    UpdatedTime = GETDATE()
                WHERE Id = @Id AND IsDeleted = 0", transaction, dbTransaction);
                if (affected == 0)
                {
                    dbTransaction.Rollback();
                    return false;
                }
                dbTransaction.Commit();
                return true;
            }
            catch
            {
                dbTransaction.Rollback();
                return false;
            }
        }

        public bool DeleteStockTransaction(int id)
        {
            using var conn = _context.GetConnection();
            if (conn.State != ConnectionState.Open)
                conn.Open();
            using var dbTransaction = conn.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                var existing = conn.QueryFirstOrDefault<StockTransaction>(
                    "SELECT * FROM StockTransactions WITH (UPDLOCK, HOLDLOCK) WHERE Id = @Id AND IsDeleted = 0",
                    new { Id = id }, dbTransaction);
                if (existing == null)
                {
                    dbTransaction.Rollback();
                    return false;
                }

                var quantityChange = -GetStockQuantityDelta(existing);
                conn.Execute(@"UPDATE ProductInventories SET Quantity = Quantity + @QuantityChange, UpdatedTime = GETDATE()
                               WHERE ProductId = @ProductId", new { existing.ProductId, QuantityChange = quantityChange }, dbTransaction);
                var affected = conn.Execute("UPDATE StockTransactions SET IsDeleted = 1 WHERE Id = @Id AND IsDeleted = 0",
                    new { Id = id }, dbTransaction);
                if (affected == 0)
                {
                    dbTransaction.Rollback();
                    return false;
                }
                dbTransaction.Commit();
                return true;
            }
            catch
            {
                dbTransaction.Rollback();
                return false;
            }
        }

        public bool UpdateStockInventory(int productId, int quantityChange, decimal averageCost)
        {
            using var conn = _context.GetConnection();
            if (conn.State != ConnectionState.Open)
                conn.Open();
            using var dbTransaction = conn.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                UpdateInventoryInTransaction(conn, dbTransaction, productId, quantityChange);
                dbTransaction.Commit();
                return true;
            }
            catch
            {
                dbTransaction.Rollback();
                return false;
            }
        }

        private static int GetStockQuantityDelta(StockTransaction transaction) => transaction.TransactionType?.Trim().ToLowerInvariant() switch
        {
            "import" or "restock" or "reimport" => Math.Abs(transaction.Quantity),
            "export" or "return" => -Math.Abs(transaction.Quantity),
            _ => transaction.Quantity
        };

        private static void UpdateInventoryInTransaction(System.Data.IDbConnection conn, IDbTransaction dbTransaction, int productId, int quantityChange)
        {
            var inventoryExists = conn.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM ProductInventories WITH (UPDLOCK, HOLDLOCK) WHERE ProductId = @ProductId",
                new { ProductId = productId }, dbTransaction) > 0;
            if (inventoryExists)
            {
                conn.Execute(@"UPDATE ProductInventories SET Quantity = Quantity + @QuantityChange, UpdatedTime = GETDATE()
                               WHERE ProductId = @ProductId", new { ProductId = productId, QuantityChange = quantityChange }, dbTransaction);
            }
            else
            {
                conn.Execute(@"INSERT INTO ProductInventories (ProductId, Quantity, LowStockThreshold, UpdatedTime)
                               VALUES (@ProductId, @Quantity, 5, GETDATE())",
                    new { ProductId = productId, Quantity = quantityChange }, dbTransaction);
            }
        }

        public List<StockInventory> ListStockInventories()
        {
            using var conn = _context.GetConnection();
            return conn.Query<StockInventory>(
                @"SELECT pi.ProductId, p.ProductName, pi.Quantity as CurrentQuantity, pi.LowStockThreshold, pi.UpdatedTime as LastUpdated, GETDATE() as CreatedTime, COALESCE(p.ImportPrice, 0) as AverageCost, 1000 as MaxStockThreshold
                FROM ProductInventories pi
                LEFT JOIN Products p ON pi.ProductId = p.Id
                ORDER BY pi.Quantity ASC").ToList();
        }

        public List<StockInventory> ListStockInventories(int page, int pageSize, out int totalCount)
        {
            using var conn = _context.GetConnection();
            var safePage = Math.Max(1, page);
            var safePageSize = Math.Clamp(pageSize, 1, 100);
            using var result = conn.QueryMultiple(@"
                SELECT COUNT(1)
                FROM ProductInventories;

                SELECT pi.ProductId, p.ProductName, pi.Quantity as CurrentQuantity, pi.LowStockThreshold,
                       pi.UpdatedTime as LastUpdated, GETDATE() as CreatedTime,
                       COALESCE(p.ImportPrice, 0) as AverageCost, 1000 as MaxStockThreshold
                FROM ProductInventories pi
                LEFT JOIN Products p ON pi.ProductId = p.Id
                ORDER BY pi.Quantity ASC, p.ProductName ASC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;",
                new { Offset = (safePage - 1) * safePageSize, PageSize = safePageSize });

            totalCount = result.ReadFirst<int>();
            return result.Read<StockInventory>().ToList();
        }

        public (List<StockInventory> LowStock, List<StockInventory> OutOfStock) GetInventoryAlerts(int take)
        {
            using var conn = _context.GetConnection();
            var safeTake = Math.Clamp(take, 0, 100);
            var lowStock = conn.Query<StockInventory>(@"
                SELECT pi.ProductId, p.ProductName, pi.Quantity AS CurrentQuantity, pi.LowStockThreshold,
                       pi.UpdatedTime AS LastUpdated, GETDATE() AS CreatedTime,
                       COALESCE(p.ImportPrice, 0) AS AverageCost, 1000 AS MaxStockThreshold
                FROM ProductInventories pi
                LEFT JOIN Products p ON p.Id = pi.ProductId
                WHERE pi.Quantity > 0 AND pi.Quantity <= pi.LowStockThreshold
                ORDER BY pi.Quantity, p.ProductName OFFSET 0 ROWS FETCH NEXT @Take ROWS ONLY", new { Take = safeTake }).ToList();
            var outOfStock = conn.Query<StockInventory>(@"
                SELECT pi.ProductId, p.ProductName, pi.Quantity AS CurrentQuantity, pi.LowStockThreshold,
                       pi.UpdatedTime AS LastUpdated, GETDATE() AS CreatedTime,
                       COALESCE(p.ImportPrice, 0) AS AverageCost, 1000 AS MaxStockThreshold
                FROM ProductInventories pi LEFT JOIN Products p ON p.Id = pi.ProductId
                WHERE pi.Quantity = 0
                ORDER BY p.ProductName OFFSET 0 ROWS FETCH NEXT @Take ROWS ONLY", new { Take = safeTake }).ToList();
            return (lowStock, outOfStock);
        }

        public List<StockTransaction> ListRecentStockTransactions(int take, DateTime fromDate, DateTime toDate)
        {
            using var conn = _context.GetConnection();
            return conn.Query<StockTransaction>(@"
                SELECT TOP (@Take) st.*, p.ProductName
                FROM StockTransactions st LEFT JOIN Products p ON p.Id = st.ProductId
                WHERE st.IsDeleted = 0 AND st.TransactionDate >= @FromDate AND st.TransactionDate <= @ToDate
                ORDER BY st.TransactionDate DESC", new { Take = Math.Clamp(take, 0, 100), FromDate = fromDate, ToDate = toDate }).ToList();
        }

        public List<StockTopExportData> ListTopExportProducts(DateTime fromDate, DateTime toDate, int take)
        {
            using var conn = _context.GetConnection();
            return conn.Query<StockTopExportData>(@"
                SELECT TOP (@Take) st.ProductId, COALESCE(p.ProductName, CONCAT(N'Sản phẩm #', st.ProductId)) AS ProductName,
                       SUM(ABS(st.Quantity)) AS Quantity
                FROM StockTransactions st LEFT JOIN Products p ON p.Id = st.ProductId
                WHERE st.IsDeleted = 0 AND st.TransactionType = N'Export'
                  AND st.TransactionDate >= @FromDate AND st.TransactionDate <= @ToDate
                GROUP BY st.ProductId, p.ProductName
                ORDER BY SUM(ABS(st.Quantity)) DESC, st.ProductId", new { Take = Math.Clamp(take, 0, 100), FromDate = fromDate, ToDate = toDate }).ToList();
        }

        public StockDashboardData GetStockDashboardData(DateTime fromDate, DateTime toDate)
        {
            using var conn = _context.GetConnection();
            return conn.QuerySingle<StockDashboardData>(@"
                SELECT COUNT(1) AS TransactionCount,
                       COALESCE(SUM(CASE WHEN TransactionType IN (N'Import', N'Restock', N'Reimport') THEN 1 ELSE 0 END), 0) AS ImportCount,
                       COALESCE(SUM(CASE WHEN TransactionType = N'Export' THEN 1 ELSE 0 END), 0) AS ExportCount
                FROM StockTransactions WHERE IsDeleted = 0 AND TransactionDate >= @FromDate AND TransactionDate <= @ToDate",
                new { FromDate = fromDate, ToDate = toDate });
        }

        public StockInventorySummary GetStockInventorySummary()
        {
            using var conn = _context.GetConnection();
            return conn.QuerySingle<StockInventorySummary>(@"
                SELECT
                    COUNT(1) AS ProductCount,
                    COALESCE(SUM(pi.Quantity), 0) AS TotalQuantity,
                    COALESCE(SUM(CASE WHEN pi.Quantity > 0 AND pi.Quantity <= pi.LowStockThreshold THEN 1 ELSE 0 END), 0) AS LowStockCount,
                    COALESCE(SUM(CASE WHEN pi.Quantity = 0 THEN 1 ELSE 0 END), 0) AS OutOfStockCount,
                    COALESCE(SUM(CASE WHEN pi.Quantity > 0 THEN pi.Quantity * COALESCE(p.ImportPrice, 0) ELSE 0 END), 0) AS InventoryValue
                FROM ProductInventories pi
                LEFT JOIN Products p ON pi.ProductId = p.Id");
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

        public CustomerDebt? GetCustomerDebtByOrderId(int orderId)
        {
            using var conn = _context.GetConnection();
            return conn.QueryFirstOrDefault<CustomerDebt>(
                "SELECT * FROM CustomerDebts WHERE OrderId = @OrderId AND IsDeleted = 0",
                new { OrderId = orderId });
        }

        public bool CreateDebtFromOrder(CustomerDebt debt, CustomerDebtTransaction initialDebit)
        {
            if (debt.OrderId is not > 0 || debt.CustomerId <= 0 || debt.DebtAmount <= 0 ||
                initialDebit.Amount != debt.DebtAmount || initialDebit.BalanceAfter != debt.DebtAmount)
            {
                return false;
            }

            var executionStrategy = _context.Database.CreateExecutionStrategy();
            return executionStrategy.Execute(() =>
            {
                using var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable);
                var connection = _context.Database.GetDbConnection();
                var dbTransaction = transaction.GetDbTransaction();

                try
                {
                    var orderIsValid = connection.QuerySingle<bool>(@"
                        SELECT CONVERT(bit, CASE WHEN EXISTS
                        (
                            SELECT 1 FROM dbo.Orders WITH (UPDLOCK, HOLDLOCK)
                            WHERE Id = @OrderId AND CustomerId = @CustomerId
                              AND Status = 4 AND PaymentMethod = 1 AND IsDeleted = 0
                        ) AND EXISTS
                        (
                            SELECT 1 FROM dbo.Customers
                            WHERE Id = @CustomerId AND IsDeleted = 0
                        ) THEN 1 ELSE 0 END)",
                        new { OrderId = debt.OrderId.Value, debt.CustomerId }, dbTransaction);

                    if (!orderIsValid)
                    {
                        transaction.Rollback();
                        return false;
                    }

                    var debtAlreadyExists = connection.QueryFirstOrDefault<int?>(@"
                        SELECT TOP (1) Id FROM dbo.CustomerDebts WITH (UPDLOCK, HOLDLOCK)
                        WHERE OrderId = @OrderId",
                        new { OrderId = debt.OrderId.Value }, dbTransaction);
                    if (debtAlreadyExists.HasValue)
                    {
                        transaction.Rollback();
                        return false;
                    }

                    var debtId = connection.QuerySingle<int>(@"
                        INSERT INTO dbo.CustomerDebts
                            (CustomerId, OrderId, OrderCode, OrderAmount, DebtAmount, PaidAmount, PaymentReference,
                             DueDate, PaidDate, Status, Notes, CreatedTime, UpdatedTime, IsDeleted)
                        VALUES
                            (@CustomerId, @OrderId, @OrderCode, @OrderAmount, @DebtAmount, @PaidAmount, @PaymentReference,
                             @DueDate, @PaidDate, @Status, @Notes, @CreatedTime, @UpdatedTime, @IsDeleted);
                        SELECT CAST(SCOPE_IDENTITY() AS int);", debt, dbTransaction);

                    connection.Execute(@"
                        INSERT INTO dbo.CustomerDebtTransactions
                            (CustomerDebtId, OrderId, TransactionType, Amount, BalanceAfter, Reference, Description,
                             TransactionDate, CreatedTime, CreatedBy, IsDeleted)
                        VALUES
                            (@CustomerDebtId, @OrderId, N'DEBIT', @Amount, @BalanceAfter, @Reference, @Description,
                             @TransactionDate, @CreatedTime, @CreatedBy, @IsDeleted);",
                        new
                        {
                            CustomerDebtId = debtId,
                            OrderId = debt.OrderId.Value,
                            initialDebit.Amount,
                            initialDebit.BalanceAfter,
                            initialDebit.Reference,
                            initialDebit.Description,
                            initialDebit.TransactionDate,
                            initialDebit.CreatedTime,
                            initialDebit.CreatedBy,
                            initialDebit.IsDeleted
                        }, dbTransaction);

                    transaction.Commit();
                    return true;
                }
                catch (SqlException exception) when (IsUniqueConstraintViolation(exception))
                {
                    TryRollback(transaction);
                    return false;
                }
                catch
                {
                    TryRollback(transaction);
                    throw;
                }
            });
        }

        private static bool IsUniqueConstraintViolation(SqlException exception)
        {
            return exception.Number is 2601 or 2627;
        }

        private static void TryRollback(IDbContextTransaction transaction)
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // Preserve the original database exception; disposal also rolls back
                // any transaction that is still active.
            }
        }

        public List<CustomerDebtTransaction> ListDebtTransactions(int customerDebtId)
        {
            using var conn = _context.GetConnection();
            return conn.Query<CustomerDebtTransaction>(@"
                SELECT * FROM CustomerDebtTransactions
                WHERE CustomerDebtId = @CustomerDebtId AND IsDeleted = 0
                ORDER BY TransactionDate DESC", new { CustomerDebtId = customerDebtId }).ToList();
        }

        public int AddDebtTransaction(CustomerDebtTransaction transaction)
        {
            using var conn = _context.GetConnection();
            return conn.ExecuteScalar<int>(@"
                INSERT INTO CustomerDebtTransactions
                    (CustomerDebtId, OrderId, TransactionType, Amount, BalanceAfter, Reference, Description,
                     TransactionDate, CreatedTime, CreatedBy, IsDeleted)
                VALUES
                    (@CustomerDebtId, @OrderId, @TransactionType, @Amount, @BalanceAfter, @Reference, @Description,
                     @TransactionDate, @CreatedTime, @CreatedBy, @IsDeleted);
                SELECT CAST(SCOPE_IDENTITY() AS int);", transaction);
        }

        public bool ProcessDebtPayment(int debtId, decimal paymentAmount, int? staffId, string reference)
        {
            if (paymentAmount <= 0 || string.IsNullOrWhiteSpace(reference))
                return false;

            using var conn = _context.GetConnection();
            using var transaction = conn.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                var debt = conn.QueryFirstOrDefault<CustomerDebt>(@"
                    SELECT * FROM CustomerDebts WITH (UPDLOCK, HOLDLOCK)
                    WHERE Id = @DebtId AND IsDeleted = 0", new { DebtId = debtId }, transaction);
                if (debt == null)
                {
                    transaction.Rollback();
                    return false;
                }

                // A retried form submission with the same key is successful but does not
                // add a second receipt.
                var existingReceipt = conn.QueryFirstOrDefault<int?>(@"
                    SELECT TOP (1) Id FROM CustomerDebtTransactions WITH (UPDLOCK, HOLDLOCK)
                    WHERE CustomerDebtId = @DebtId AND Reference = @Reference
                      AND TransactionType = N'CREDIT' AND IsDeleted = 0",
                    new { DebtId = debtId, Reference = reference }, transaction);
                if (existingReceipt.HasValue)
                {
                    transaction.Commit();
                    return true;
                }

                var remaining = debt.DebtAmount - debt.PaidAmount;
                if (remaining <= 0 || paymentAmount > remaining)
                {
                    transaction.Rollback();
                    return false;
                }

                var newPaidAmount = debt.PaidAmount + paymentAmount;
                var newBalance = debt.DebtAmount - newPaidAmount;
                var now = DateTime.Now;
                var newStatus = newBalance == 0
                    ? "Paid"
                    : newPaidAmount > 0
                        ? "PartiallyPaid"
                        : debt.DueDate.Date < now.Date ? "Overdue" : "Pending";

                conn.Execute(@"
                    UPDATE CustomerDebts
                    SET PaidAmount = @PaidAmount, PaidDate = @PaidDate, Status = @Status,
                        UpdatedTime = @UpdatedTime
                    WHERE Id = @Id AND IsDeleted = 0;
                    INSERT INTO CustomerDebtTransactions
                        (CustomerDebtId, OrderId, TransactionType, Amount, BalanceAfter, Reference,
                         Description, TransactionDate, CreatedTime, CreatedBy, IsDeleted)
                    VALUES
                        (@Id, @OrderId, N'CREDIT', @Amount, @BalanceAfter, @Reference,
                         N'Thu tiền công nợ', @TransactionDate, @CreatedTime, @CreatedBy, 0);
                    IF @OrderId IS NOT NULL
                        UPDATE Orders SET PaymentStatus = CASE WHEN @BalanceAfter = 0 THEN 1 ELSE 3 END,
                            PaidAt = CASE WHEN @BalanceAfter = 0 THEN @TransactionDate ELSE PaidAt END
                        WHERE Id = @OrderId AND IsDeleted = 0;",
                    new
                    {
                        Id = debt.Id,
                        debt.OrderId,
                        PaidAmount = newPaidAmount,
                        PaidDate = newBalance == 0 ? now : debt.PaidDate,
                        Status = newStatus,
                        UpdatedTime = now,
                        Amount = paymentAmount,
                        BalanceAfter = newBalance,
                        Reference = reference,
                        TransactionDate = now,
                        CreatedTime = now,
                        CreatedBy = staffId
                    }, transaction);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
