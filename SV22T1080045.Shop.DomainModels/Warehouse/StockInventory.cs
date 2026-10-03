namespace SV22T1080045.Shop.DomainModels.Warehouse
{
    public class StockInventory
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; } // Product name from join
        
        public int CurrentQuantity { get; set; }
        public int LowStockThreshold { get; set; } = 5;
        public int MaxStockThreshold { get; set; } = 1000;
        
        public decimal AverageCost { get; set; } // FIFO or weighted average cost
        public decimal TotalValue => CurrentQuantity * AverageCost;
        
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedTime { get; set; }
        
        public bool IsLowStock => CurrentQuantity <= LowStockThreshold;
        public bool IsOverStock => CurrentQuantity >= MaxStockThreshold;
        public bool IsOutOfStock => CurrentQuantity == 0;
    }
}