using SV22T1080045.Shop.DomainModels.Warehouse;
using SV22T1080045.Shop.Abstractions.Models.Warehouse;
using SV22T1080045.Shop.Abstractions.Contracts.Warehouse;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface IWarehouseService
    {
        // Stock Transaction methods
        List<StockTransaction> ListStockTransactions(int productId, DateTime? fromDate, DateTime? toDate, string? transactionType = null);
        StockTransaction? GetStockTransaction(int id);
        int AddStockTransaction(StockTransactionRequest request);
        bool UpdateStockTransaction(StockTransaction transaction);
        bool DeleteStockTransaction(int id);
        List<ReimportOrderOption> ListOrdersForReimport();
        List<ReimportOrderLine> ListReimportOrderLines(int orderId);
        bool AddReimportTransaction(ReimportStockRequest request, out string error);
        
        // Stock Inventory methods
        List<StockInventory> ListStockInventories();
        List<StockInventory> ListStockInventories(int page, int pageSize, out int totalCount);
        (List<StockInventory> LowStock, List<StockInventory> OutOfStock) GetInventoryAlerts(int take);
        List<StockTransaction> ListRecentStockTransactions(int take, DateTime fromDate, DateTime toDate);
        List<StockTopExportData> ListTopExportProducts(DateTime fromDate, DateTime toDate, int take);
        StockDashboardData GetStockDashboardData(DateTime fromDate, DateTime toDate);
        StockInventorySummary GetStockInventorySummary();
        StockInventory? GetStockInventory(int productId);
        bool UpdateStockInventory(int productId, int quantityChange, decimal averageCost);
        
        // Customer Debt methods
        List<CustomerDebt> ListCustomerDebts(int? customerId);
        CustomerDebt? GetCustomerDebt(int id);
        CustomerDebt? GetCustomerDebtByOrderId(int orderId);
        int AddCustomerDebt(CustomerDebtRequest request);
        bool UpdateCustomerDebt(CustomerDebt debt);
        bool DeleteCustomerDebt(int id);
        bool ProcessDebtPayment(int debtId, decimal paymentAmount, int? staffId = null, string? reference = null);
        bool CreateDebtFromOrder(int orderId, int customerId, decimal amount, int? staffId = null);
    }
}
