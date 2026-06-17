using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Models.Management
{
    public class ManagementOrderData
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public string CustomerName { get; set; } = "";
        public string CustomerPhone { get; set; } = "";
        public int ItemCount { get; set; }
        public bool IsGuest { get; set; }
    }

    public class CustomerManagementData
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = "Customer";
        public DateTime CreatedTime { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }

    public class CustomerPurchaseHistoryData
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public int ItemCount { get; set; }
        public List<string> ProductNames { get; set; } = new();
    }

    public class ManagementOrderDetailsData
    {
        public ManagementOrderDetailHeaderData Order { get; set; } = new();
        public List<ManagementOrderDetailLineData> Details { get; set; } = new();
    }

    public class ManagementOrderDetailHeaderData
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public string CustomerName { get; set; } = "";
        public string CustomerPhone { get; set; } = "";
        public string ShippingAddress { get; set; } = "";
        public bool IsGuest { get; set; }
    }

    public class ManagementOrderDetailLineData
    {
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class VoucherSaveResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public Voucher? Voucher { get; set; }
    }

    public class VoucherToggleResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public Voucher? Voucher { get; set; }
    }
}
