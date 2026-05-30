namespace SV22T1080045.Shop.Models.ViewModels.Product
{
    /// <summary>
    /// ViewModel dùng cho trang danh sách sản phẩm /san-pham.
    /// </summary>
    public class ProductCardViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = "";
        public string? ImageUrl { get; set; }
        public string? Origin { get; set; }
        public decimal SalePrice { get; set; }
        public decimal DiscountPercentValue { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public int? Rating { get; set; }
        public int? ReviewCount { get; set; }
        public int? SoldCount { get; set; }
        public int? Quantity { get; set; }
        public int LowStockThreshold { get; set; } = 5;
        public string? UsageTags { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedTime { get; set; }

        public decimal DisplayOriginalPrice => SalePrice > 0 ? SalePrice : OriginalPrice;

        public decimal DisplayPrice => PriceAfterDiscount > 0
            ? PriceAfterDiscount
            : Math.Round(DisplayOriginalPrice * (100 - DiscountPercentValue) / 100, 0);

        public bool HasDiscount => DisplayOriginalPrice > DisplayPrice;

        public int DiscountPercent =>
            HasDiscount && DisplayOriginalPrice > 0
                ? (int)Math.Round((1 - (double)DisplayPrice / (double)DisplayOriginalPrice) * 100)
                : 0;

        public bool InStock => Quantity > 0;
        public bool IsLowStock => Quantity > 0 && Quantity <= LowStockThreshold;
        public string StockStatusText => !InStock ? "Hết hàng" : IsLowStock ? "Sắp hết" : "Còn hàng";
        public string StockStatusClass => !InStock ? "out" : IsLowStock ? "low" : "in";
        public bool IsBestSeller => (SoldCount ?? 0) >= 50;
        public bool IsNew => CreatedTime >= DateTime.Now.AddDays(-30);
    }
}
