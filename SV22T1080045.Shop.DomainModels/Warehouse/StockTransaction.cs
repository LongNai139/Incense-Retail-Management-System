namespace SV22T1080045.Shop.DomainModels.Warehouse
{
    public class StockTransaction
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; } // Product name from join
        
        public int Quantity { get; set; } // Positive for import, negative for export
        public string TransactionType { get; set; } = "Import"; // Import, Export, Restock, Return
        
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount => Quantity * UnitPrice;
        
        public string? ReferenceNumber { get; set; } // Order ID, invoice number, etc.
        public string? Reason { get; set; }
        
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedTime { get; set; }
        
        public int? CreatedBy { get; set; }
        
        public bool IsDeleted { get; set; }
    }
    
    public enum StockTransactionType
    {
        Import = 1,      // Nhập kho mới
        Export = 2,     // Xuất kho bán hàng
        Restock = 3,     // Tái nhập kho
        Return = 4,      // Trả hàng
        Adjustment = 5   // Điều chỉnh tồn kho
    }
}