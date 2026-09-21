using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Models.ViewModels.Staff
{
    public class StaffDashboardViewModel
    {
        public int NewOrdersToday { get; set; }
        public int ProcessingOrders { get; set; }
        public int OrdersToDeliver { get; set; }
        public int LowStockProducts { get; set; }

        public string OrderSearchValue { get; set; } = "";
        public int? OrderStatus { get; set; }
        public DateTime WorkDate { get; set; }
        public string ProductSearchValue { get; set; } = "";
        public int ProductPage { get; set; } = 1;
        public int ProductPageSize { get; set; } = 12;
        public int ProductTotalItems { get; set; }
        public int ProductTotalPages { get; set; } = 1;
        public int ProductStartItem { get; set; }
        public int ProductEndItem { get; set; }

        public List<StaffOrderRowViewModel> RecentOrders { get; set; } = new();
        public List<StaffOrderRowViewModel> Orders { get; set; } = new();
        public List<StaffProductRowViewModel> Products { get; set; } = new();
        public List<StaffCustomerRowViewModel> Customers { get; set; } = new();
        public List<StaffAlertViewModel> Alerts { get; set; } = new();
        public List<StaffNotificationViewModel> Notifications { get; set; } = new();
        public StaffProfileViewModel Profile { get; set; } = new();
    }

    public class StaffOrderRowViewModel
    {
        public int Id { get; set; }
        public string OrderCode => $"#TH{Id:D6}";
        public string CustomerName { get; set; } = "";
        public string CustomerPhone { get; set; } = "";
        public string ShippingAddress { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public DateTime OrderDate { get; set; }
        public int ItemCount { get; set; }
        public bool IsGuest { get; set; }
        public List<StaffOrderItemViewModel> Items { get; set; } = new();
    }

    public class StaffOrderItemViewModel
    {
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
    }

    public class StaffProductRowViewModel
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

    public class StaffCustomerRowViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = "";
        public string MaskedPhone { get; set; } = "";
        public string MaskedEmail { get; set; } = "";
        public int OrderCount { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public List<string> RecentProducts { get; set; } = new();
    }

    public class StaffAlertViewModel
    {
        public string Title { get; set; } = "";
        public string Detail { get; set; } = "";
        public string Level { get; set; } = "info";
        public string Icon { get; set; } = "fa-circle-info";
    }

    public class StaffNotificationViewModel
    {
        public string Message { get; set; } = "";
        public string Detail { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string Type { get; set; } = "info";
        public string? TargetAnchor { get; set; }
    }

    public class StaffProfileViewModel
    {
        public string DisplayName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Role { get; set; } = "Staff";
        public string AvatarInitials { get; set; } = "ST";
    }
}
