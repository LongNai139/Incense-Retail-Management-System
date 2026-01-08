namespace SV22T1080045.Shop.DomainModels
{
    public class OrderDetails
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public string ProductName { get; set; } = "";
        public string Photo { get; set; } = "";  
        public decimal TotalPrice => Quantity * SalePrice;
    }
}