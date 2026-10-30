namespace SV22T1080045.Shop.Abstractions.Models.Staff
{
    public class StaffDashboardData
    {
        public List<StaffOrderData> TodayOrders { get; set; } = new();
        public List<StaffOrderData> RecentOrders { get; set; } = new();
        public List<StaffOrderData> PendingTooLongOrders { get; set; } = new();
        public List<StaffOrderData> FailedDeliveryOrders { get; set; } = new();
        public List<StaffProductData> LowStockProducts { get; set; } = new();
        public List<StaffCustomerData> Customers { get; set; } = new();
        public int NewOrdersToday { get; set; }
        public int ProcessingOrders { get; set; }
        public int OrdersToDeliver { get; set; }
        public int LowStockCount { get; set; }
    }

    public class StaffOrderDashboardData
    {
        public int NewOrdersToday { get; set; }
        public int ProcessingOrders { get; set; }
        public int OrdersToDeliver { get; set; }
        public List<StaffOrderData> RecentOrders { get; set; } = new();
        public List<StaffOrderData> PendingTooLongOrders { get; set; } = new();
        public List<StaffOrderData> FailedDeliveryOrders { get; set; } = new();
    }

    public class StaffProductPageData
    {
        public List<StaffProductData> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class StaffOrderData
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public int Status { get; set; }
        public int PaymentMethod { get; set; }
        public int PaymentStatus { get; set; }
        public string? PaymentTransactionNo { get; set; }
        public DateTime? PaidAt { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public int? CustomerDebtId { get; set; }
        public List<StaffDebtTransactionData> DebtTransactions { get; set; } = new();
        public string CustomerName { get; set; } = "";
        public string CustomerPhone { get; set; } = "";
        public string ShippingAddress { get; set; } = "";
        public bool IsGuest { get; set; }
        public int ItemCount { get; set; }
        public List<StaffOrderItemData> Items { get; set; } = new();
    }

    public class StaffDebtTransactionData
    {
        public string TransactionType { get; set; } = "";
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string? Reference { get; set; }
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class StaffOrderItemData
    {
        public int OrderId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class StaffProductData
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public string UnitName { get; set; } = "";
        public int Quantity { get; set; }
        public int SoldCount { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public string? Origin { get; set; }
    }

    public class StaffCustomerData
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string? Email { get; set; }
        public int OrderCount { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public List<string> RecentProducts { get; set; } = new();
    }

    public class StaffOperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public List<InventoryCheckResult> InventoryChecks { get; set; } = new();
    }

    public class InventoryCheckResult
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int RequestedQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public bool IsSufficient { get; set; }
    }
}
