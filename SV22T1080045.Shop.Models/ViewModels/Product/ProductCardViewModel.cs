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

        /// <summary>Giá bán niêm yết trước giảm (giống prototype: originalPrice).</summary>
        public decimal DisplayOriginalPrice => SalePrice > 0 ? SalePrice : OriginalPrice;

        /// <summary>Giá khách mua (giống prototype: price).</summary>
        public decimal DisplayPrice => PriceAfterDiscount;

        public bool HasDiscount => DisplayOriginalPrice > DisplayPrice && DisplayPrice > 0;

        public int DiscountPercent
        {
            get
            {
                if (!HasDiscount || DisplayOriginalPrice <= 0)
                    return 0;

                var percent = DiscountPercentValue > 0
                    ? DiscountPercentValue
                    : (decimal)((1 - (double)DisplayPrice / (double)DisplayOriginalPrice) * 100);

                return (int)Math.Round(percent);
            }
        }

        public bool InStock => Quantity > 0;
        public bool IsLowStock => Quantity > 0 && Quantity <= LowStockThreshold;
        public string StockStatusText => !InStock ? "Hết hàng" : IsLowStock ? "Sắp hết" : "Còn hàng";
        public string StockStatusClass => !InStock ? "out" : IsLowStock ? "low" : "in";
        public bool IsBestSeller => (SoldCount ?? 0) >= 50;
        public bool IsNew => CreatedTime >= DateTime.Now.AddDays(-30);
    }
}
