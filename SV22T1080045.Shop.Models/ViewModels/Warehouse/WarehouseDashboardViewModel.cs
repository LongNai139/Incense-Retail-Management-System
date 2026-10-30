namespace SV22T1080045.Shop.Models.ViewModels.Warehouse
{
    public class WarehouseDashboardViewModel
    {
        public List<WarehouseInventoryRowViewModel> StockInventories { get; set; } = new();
        public List<WarehouseInventoryRowViewModel> LowStockProducts { get; set; } = new();
        public List<WarehouseInventoryRowViewModel> OutOfStockProducts { get; set; } = new();
        public List<WarehouseTransactionRowViewModel> RecentTransactions { get; set; } = new();
        public List<WarehouseTopExportViewModel> TopExportProducts { get; set; } = new();
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public decimal TotalDebt { get; set; }
        public int OverdueDebtCount { get; set; }
        public decimal InventoryValue { get; set; }
        public int TotalStockQuantity { get; set; }
        public int InventoryPage { get; set; } = 1;
        public int InventoryPageSize { get; set; } = 10;
        public int InventoryTotalCount { get; set; }
        public int InventoryTotalPages { get; set; } = 1;
        public int TransactionCountLast30Days { get; set; }
        public int ImportCountLast30Days { get; set; }
        public int ExportCountLast30Days { get; set; }
        public bool CanImportInventory { get; set; }
        public bool CanExportInventory { get; set; }
    }

    public class WarehouseInventoryRowViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int CurrentQuantity { get; set; }
        public int LowStockThreshold { get; set; }
        public decimal EstimatedValue { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsLowStock => CurrentQuantity > 0 && CurrentQuantity <= LowStockThreshold;
        public bool IsOutOfStock => CurrentQuantity == 0;
    }

    public class WarehouseTransactionRowViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public string TransactionType { get; set; } = "";
        public string? ReferenceNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public int? CreatedBy { get; set; }
    }

    public class WarehouseTopExportViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
    }

    public class StockTransactionPageViewModel
    {
        public List<StockTransactionRowViewModel> Transactions { get; set; } = new();
        public int? ProductId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string TransactionType { get; set; } = "Import";
        public List<WarehouseOptionViewModel> AvailableProducts { get; set; } = new();
        public List<WarehouseOptionViewModel> ReimportOrders { get; set; } = new();
        public int? ReimportOrderId { get; set; }
        public List<ReimportOrderLineViewModel> ReimportLines { get; set; } = new();
    }

    public class WarehouseOptionViewModel
    {
        public string Value { get; set; } = "";
        public string Text { get; set; } = "";
        public bool Selected { get; set; }
    }

    public class StockTransactionRowViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public string TransactionType { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public int? CreatedBy { get; set; }
    }

    public class StockTransactionInputViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string TransactionType { get; set; } = "Import";
        public decimal UnitPrice { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Reason { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class ReimportOrderLineViewModel
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int ExportedQuantity { get; set; }
        public int ReturnedQuantity { get; set; }
        public int ReturnableQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ImportPrice { get; set; }
    }

    public class ReimportStockInputViewModel
    {
        public int OrderId { get; set; }
        public int OrderDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Reason { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class CustomerDebtPageViewModel
    {
        public List<CustomerDebtRowViewModel> Debts { get; set; } = new();
        public int? CustomerId { get; set; }
    }

    public class CustomerDebtRowViewModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? OrderCode { get; set; }
        public decimal DebtAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount => DebtAmount - PaidAmount;
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = "Pending";
    }

    public class CustomerDebtInputViewModel
    {
        public int CustomerId { get; set; }
        public string? OrderCode { get; set; }
        public decimal DebtAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string? PaymentReference { get; set; }
        public DateTime DueDate { get; set; }
        public string? Notes { get; set; }
    }
}
