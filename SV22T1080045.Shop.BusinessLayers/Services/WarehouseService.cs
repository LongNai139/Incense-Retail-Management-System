using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models.Warehouse;
using SV22T1080045.Shop.Abstractions.Contracts.Warehouse;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels.Warehouse;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseDAL _warehouseDAL;

        public WarehouseService(IWarehouseDAL warehouseDAL)
        {
            _warehouseDAL = warehouseDAL;
        }

        public List<StockTransaction> ListStockTransactions(int productId, DateTime? fromDate, DateTime? toDate, string? transactionType = null)
        {
            return _warehouseDAL.ListStockTransactions(productId, fromDate, toDate, transactionType?.Trim());
        }

        public StockTransaction? GetStockTransaction(int id)
        {
            return _warehouseDAL.GetStockTransaction(id);
        }

        public int AddStockTransaction(StockTransactionRequest request)
        {
            var transaction = new StockTransaction
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                TransactionType = request.TransactionType?.Trim() ?? "",
                UnitPrice = request.UnitPrice,
                ReferenceNumber = request.ReferenceNumber?.Trim(),
                Reason = request.Reason?.Trim(),
                TransactionDate = request.TransactionDate
            };
            if (string.Equals(transaction.TransactionType, "Reimport", StringComparison.OrdinalIgnoreCase))
                return 0;

            transaction.CreatedTime = DateTime.Now;
            transaction.TransactionDate = transaction.TransactionDate == default ? DateTime.Now : transaction.TransactionDate;
            transaction.IsDeleted = false;
            
            return _warehouseDAL.AddStockTransaction(transaction);
        }

        public List<ReimportOrderOption> ListOrdersForReimport() => _warehouseDAL.ListOrdersForReimport();

        public List<ReimportOrderLine> ListReimportOrderLines(int orderId) => _warehouseDAL.ListReimportOrderLines(orderId);

        public bool AddReimportTransaction(ReimportStockRequest request, out string error)
        {
            if (request.Quantity <= 0)
            {
                error = "Số lượng tái nhập phải lớn hơn 0.";
                return false;
            }
            return _warehouseDAL.AddReimportTransaction(new StockTransaction
            {
                Quantity = request.Quantity,
                Reason = request.Reason?.Trim(),
                TransactionDate = request.TransactionDate
            }, request.OrderId, request.OrderDetailId, out error);
        }

        public bool UpdateStockTransaction(StockTransaction transaction)
        {
            var existing = _warehouseDAL.GetStockTransaction(transaction.Id);
            if (existing == null)
                return false;
            
            return _warehouseDAL.UpdateStockTransaction(transaction);
        }

        public bool DeleteStockTransaction(int id)
        {
            var existing = _warehouseDAL.GetStockTransaction(id);
            if (existing == null)
                return false;
            
            return _warehouseDAL.DeleteStockTransaction(id);
        }

        public List<StockInventory> ListStockInventories()
        {
            return _warehouseDAL.ListStockInventories();
        }

        public List<StockInventory> ListStockInventories(int page, int pageSize, out int totalCount)
        {
            return _warehouseDAL.ListStockInventories(page, pageSize, out totalCount);
        }

        public (List<StockInventory> LowStock, List<StockInventory> OutOfStock) GetInventoryAlerts(int take)
            => _warehouseDAL.GetInventoryAlerts(Math.Clamp(take, 0, 100));

        public List<StockTransaction> ListRecentStockTransactions(int take, DateTime fromDate, DateTime toDate)
            => _warehouseDAL.ListRecentStockTransactions(Math.Clamp(take, 0, 100), fromDate, toDate);

        public List<StockTopExportData> ListTopExportProducts(DateTime fromDate, DateTime toDate, int take)
            => _warehouseDAL.ListTopExportProducts(fromDate, toDate, Math.Clamp(take, 0, 100));

        public StockDashboardData GetStockDashboardData(DateTime fromDate, DateTime toDate)
            => _warehouseDAL.GetStockDashboardData(fromDate, toDate);

        public StockInventorySummary GetStockInventorySummary()
        {
            return _warehouseDAL.GetStockInventorySummary();
        }

        public StockInventory? GetStockInventory(int productId)
        {
            return _warehouseDAL.GetStockInventory(productId);
        }

        public bool UpdateStockInventory(int productId, int quantityChange, decimal averageCost)
        {
            return _warehouseDAL.UpdateStockInventory(productId, quantityChange, averageCost);
        }

        public List<CustomerDebt> ListCustomerDebts(int? customerId)
        {
            return _warehouseDAL.ListCustomerDebts(customerId);
        }

        public CustomerDebt? GetCustomerDebt(int id)
        {
            return _warehouseDAL.GetCustomerDebt(id);
        }

        public CustomerDebt? GetCustomerDebtByOrderId(int orderId)
            => _warehouseDAL.GetCustomerDebtByOrderId(orderId);

        public int AddCustomerDebt(CustomerDebtRequest request)
        {
            if (request.CustomerId <= 0 || request.DebtAmount <= 0 || request.PaidAmount < 0 || request.PaidAmount > request.DebtAmount)
                return 0;

            var debt = new CustomerDebt
            {
                CustomerId = request.CustomerId,
                OrderCode = request.OrderCode?.Trim(),
                DebtAmount = request.DebtAmount,
                PaidAmount = request.PaidAmount,
                PaymentReference = request.PaymentReference?.Trim(),
                DueDate = request.DueDate,
                Notes = request.Notes?.Trim(),
                OrderId = null,
                OrderAmount = request.DebtAmount
            };
            debt.CreatedTime = DateTime.Now;
            debt.Status = debt.PaidAmount >= debt.DebtAmount ? "Paid" : 
                          debt.PaidAmount > 0 ? "PartiallyPaid" : "Pending";
            debt.IsDeleted = false;
            
            return _warehouseDAL.AddCustomerDebt(debt);
        }

        public bool UpdateCustomerDebt(CustomerDebt debt)
        {
            debt.Status = debt.PaidAmount >= debt.DebtAmount ? "Paid" : 
                          debt.PaidAmount > 0 ? "PartiallyPaid" : "Pending";
            debt.UpdatedTime = DateTime.Now;
            
            return _warehouseDAL.UpdateCustomerDebt(debt);
        }

        public bool DeleteCustomerDebt(int id)
        {
            return _warehouseDAL.DeleteCustomerDebt(id);
        }

        public bool ProcessDebtPayment(int debtId, decimal paymentAmount, int? staffId = null, string? reference = null)
        {
            if (paymentAmount <= 0)
                return false;
            reference = string.IsNullOrWhiteSpace(reference) ? $"PAY-{Guid.NewGuid():N}" : reference.Trim();
            return _warehouseDAL.ProcessDebtPayment(debtId, paymentAmount, staffId, reference);
        }

        public bool CreateDebtFromOrder(int orderId, int customerId, decimal amount, int? staffId = null)
        {
            if (orderId <= 0 || customerId <= 0 || amount <= 0)
                return false;

            var now = DateTime.Now;
            var debt = new CustomerDebt
            {
                CustomerId = customerId,
                OrderId = orderId,
                OrderCode = $"TH{orderId:D6}",
                OrderAmount = amount,
                DebtAmount = amount,
                PaidAmount = 0,
                DueDate = DateTime.Today.AddDays(30),
                Status = "Pending",
                Notes = "Tự động tạo từ đơn hàng COD",
                CreatedTime = now,
                IsDeleted = false
            };

            var initialDebit = new CustomerDebtTransaction
            {
                OrderId = orderId,
                TransactionType = "DEBIT",
                Amount = amount,
                BalanceAfter = amount,
                Reference = $"ORDER-{orderId}",
                Description = "Phát sinh từ đơn hàng COD",
                TransactionDate = now,
                CreatedTime = now,
                CreatedBy = staffId,
                IsDeleted = false
            };

            return _warehouseDAL.CreateDebtFromOrder(debt, initialDebit);
        }
    }
}
