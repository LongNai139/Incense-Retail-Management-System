using SV22T1080045.Shop.DomainModels.Warehouse;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    public interface IWarehouseDAL
    {
        // Stock Transaction methods
        List<StockTransaction> ListStockTransactions(int productId, DateTime? fromDate, DateTime? toDate);
        StockTransaction? GetStockTransaction(int id);
        int AddStockTransaction(StockTransaction transaction);
        bool UpdateStockTransaction(StockTransaction transaction);
        bool DeleteStockTransaction(int id);
        
        // Stock Inventory methods
        List<StockInventory> ListStockInventories();
        StockInventory? GetStockInventory(int productId);
        bool UpdateStockInventory(int productId, int quantityChange, decimal averageCost);
        
        // Customer Debt methods
        List<CustomerDebt> ListCustomerDebts(int? customerId);
        CustomerDebt? GetCustomerDebt(int id);
        int AddCustomerDebt(CustomerDebt debt);
        bool UpdateCustomerDebt(CustomerDebt debt);
        bool DeleteCustomerDebt(int id);
    }
}