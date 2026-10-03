using SV22T1080045.Shop.Abstractions.Interfaces;
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

        public List<StockTransaction> ListStockTransactions(int productId, DateTime? fromDate, DateTime? toDate)
        {
            return _warehouseDAL.ListStockTransactions(productId, fromDate, toDate);
        }

        public StockTransaction? GetStockTransaction(int id)
        {
            return _warehouseDAL.GetStockTransaction(id);
        }

        public int AddStockTransaction(StockTransaction transaction)
        {
            transaction.CreatedTime = DateTime.Now;
            transaction.TransactionDate = transaction.TransactionDate == default ? DateTime.Now : transaction.TransactionDate;
            transaction.IsDeleted = false;
            
            // Calculate quantity change based on transaction type
            var quantityChange = transaction.Quantity;
            switch (transaction.TransactionType?.ToLower())
            {
                case "import":
                case "restock":
                    quantityChange = Math.Abs(transaction.Quantity); // Always positive for imports
                    break;
                case "export":
                case "return":
                    quantityChange = -Math.Abs(transaction.Quantity); // Always negative for exports
                    break;
                case "adjustment":
                    quantityChange = transaction.Quantity; // Can be positive or negative
                    break;
                default:
                    quantityChange = transaction.Quantity;
                    break;
            }
            
            var transactionId = _warehouseDAL.AddStockTransaction(transaction);
            
            // Update inventory automatically
            if (transactionId > 0)
            {
                _warehouseDAL.UpdateStockInventory(transaction.ProductId, quantityChange, transaction.UnitPrice);
            }
            
            return transactionId;
        }

        public bool UpdateStockTransaction(StockTransaction transaction)
        {
            var existing = _warehouseDAL.GetStockTransaction(transaction.Id);
            if (existing == null)
                return false;
            
            // Calculate the difference in quantity
            var quantityDifference = transaction.Quantity - existing.Quantity;
            
            var result = _warehouseDAL.UpdateStockTransaction(transaction);
            
            // Update inventory with the difference
            if (result)
            {
                _warehouseDAL.UpdateStockInventory(transaction.ProductId, quantityDifference, transaction.UnitPrice);
            }
            
            return result;
        }

        public bool DeleteStockTransaction(int id)
        {
            var existing = _warehouseDAL.GetStockTransaction(id);
            if (existing == null)
                return false;
            
            var result = _warehouseDAL.DeleteStockTransaction(id);
            
            // Reverse the inventory change
            if (result)
            {
                _warehouseDAL.UpdateStockInventory(existing.ProductId, -existing.Quantity, existing.UnitPrice);
            }
            
            return result;
        }

        public List<StockInventory> ListStockInventories()
        {
            return _warehouseDAL.ListStockInventories();
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

        public int AddCustomerDebt(CustomerDebt debt)
        {
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

        public bool ProcessDebtPayment(int debtId, decimal paymentAmount)
        {
            var debt = _warehouseDAL.GetCustomerDebt(debtId);
            if (debt == null)
                return false;
            
            debt.PaidAmount += paymentAmount;
            debt.PaidDate = debt.PaidAmount >= debt.DebtAmount ? DateTime.Now : debt.PaidDate;
            debt.Status = debt.PaidAmount >= debt.DebtAmount ? "Paid" : 
                          debt.PaidAmount > 0 ? "PartiallyPaid" : "Pending";
            debt.UpdatedTime = DateTime.Now;
            
            return _warehouseDAL.UpdateCustomerDebt(debt);
        }
    }
}