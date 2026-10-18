namespace SV22T1080045.Shop.DomainModels.Warehouse
{
    public class ReimportOrderOption
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; } = "";
    }

    public class ReimportOrderLine
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int ExportedQuantity { get; set; }
        public int ReturnedQuantity { get; set; }
        public int ReturnableQuantity => Math.Max(0, ExportedQuantity - ReturnedQuantity);
        public decimal UnitPrice { get; set; }
        public decimal ImportPrice { get; set; }
    }

    public class ReimportOrderSource
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public decimal ImportPrice { get; set; }
    }
}
