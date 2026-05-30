namespace SV22T1080045.Shop.Models.ViewModels.Product
{
    public class ProductDetailsViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = "";
        public string? Description { get; set; }
        public string? Ingredient { get; set; }
        public string? BurningTime { get; set; }
        public string? Origin { get; set; }
        public string? AgeYear { get; set; }
        public string? OilContent { get; set; }
        public string? Length { get; set; }
        public string? Weight { get; set; }
        public int? Quantity { get; set; }
        public int LowStockThreshold { get; set; } = 5;
        public int? SoldCount { get; set; }
        public int? Rating { get; set; }
        public int? ReviewCount { get; set; }
        public decimal SalePrice { get; set; }
        public decimal DiscountPercentValue { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public string? ImageUrl { get; set; }
        public string? CategoryName { get; set; }
        public string? UnitName { get; set; }
        public List<string> UsageTags { get; set; } = new();
        public List<string> GalleryImageUrls { get; set; } = new();
        public List<ProductCardViewModel> RelatedProducts { get; set; } = new();

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
        public string StockStatusClass => !InStock ? "stock-out" : IsLowStock ? "stock-low" : "stock-ok";
    }
}
