namespace SV22T1080045.Shop.Models.ViewModels.Checkout
{
    public class CheckoutSuccessViewModel
    {
        public int OrderId { get; set; }
        public string ShippingName { get; set; } = "";
        public string ShippingPhone { get; set; } = "";
        public string ShippingAddress { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public string? VoucherCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public int PaymentMethod { get; set; }
        public int PaymentStatus { get; set; }
        public string PaymentMethodText => PaymentMethod switch
        {
            2 => "VNPay",
            3 => "MoMo",
            _ => "COD"
        };
        public string PaymentStatusText => PaymentStatus switch
        {
            1 => "Đã thanh toán",
            2 => "Thanh toán thất bại",
            _ => "Chưa thanh toán"
        };
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
