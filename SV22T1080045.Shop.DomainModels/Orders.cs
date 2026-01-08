namespace SV22T1080045.Shop.DomainModels
{
    public class Orders
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderTime { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? DeliveryPhone { get; set; }
        public string? Status { get; set; }
        public decimal TotalAmount { get; set; } = 0;
    }
}