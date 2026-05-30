namespace SV22T1080045.Shop.Abstractions.Models.Staff
{
    public class StaffDashboardData
    {
        public List<StaffOrderData> Orders { get; set; } = new();
        public List<StaffProductData> Products { get; set; } = new();
        public List<StaffCustomerData> Customers { get; set; } = new();
    }

    public class StaffOrderData
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public string CustomerName { get; set; } = "";
        public string CustomerPhone { get; set; } = "";
        public string ShippingAddress { get; set; } = "";
        public bool IsGuest { get; set; }
        public int ItemCount { get; set; }
        public List<StaffOrderItemData> Items { get; set; } = new();
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
    }
}
