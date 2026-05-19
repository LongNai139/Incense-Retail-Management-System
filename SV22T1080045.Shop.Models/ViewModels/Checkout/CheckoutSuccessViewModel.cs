namespace SV22T1080045.Shop.Models.ViewModels.Checkout
{
    public class CheckoutSuccessViewModel
    {
        public int OrderId { get; set; }
        public string ShippingName { get; set; } = "";
        public string ShippingPhone { get; set; } = "";
        public string ShippingAddress { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public List<CheckoutOrderDetailViewModel> Details { get; set; } = new();
    }

    public class CheckoutOrderDetailViewModel
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
