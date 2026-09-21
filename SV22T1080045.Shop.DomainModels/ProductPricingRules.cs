namespace SV22T1080045.Shop.DomainModels
{
    /// <summary>
    /// Quy ước giá:
    /// - ImportPrice: giá nhập (chỉ quản trị, không hiển thị khách).
    /// - SalePrice / OriginalPrice (legacy): giá bán niêm yết trước giảm.
    /// - DiscountPercent: % giảm do admin thiết lập.
    /// - PriceAfterDiscount: giá khách trả (ưu tiên nếu đã có từ dữ liệu cũ).
    /// </summary>
    public static class ProductPricingRules
    {
        public static decimal GetListPrice(decimal salePrice, decimal originalPrice)
            => salePrice > 0 ? salePrice : originalPrice;

        public static decimal GetCustomerPrice(
            decimal salePrice,
            decimal originalPrice,
            decimal discountPercent,
            decimal priceAfterDiscount)
        {
            var listPrice = GetListPrice(salePrice, originalPrice);
            if (listPrice <= 0)
                return 0;

            if (discountPercent > 0)
                return Math.Round(listPrice * (100 - discountPercent) / 100, 0);

            if (priceAfterDiscount > 0 && priceAfterDiscount <= listPrice)
                return priceAfterDiscount;

            return listPrice;
        }

        public static decimal GetDiscountPercent(
            decimal listPrice,
            decimal customerPrice,
            decimal configuredPercent)
        {
            if (configuredPercent > 0)
                return configuredPercent;

            if (listPrice <= 0 || customerPrice <= 0 || customerPrice >= listPrice)
                return 0;

            return Math.Round((1 - customerPrice / listPrice) * 100, 2);
        }

        public static decimal CalculatePriceAfterDiscount(decimal listPrice, decimal discountPercent)
        {
            if (listPrice <= 0)
                return 0;

            if (discountPercent <= 0)
                return listPrice;

            return Math.Round(listPrice * (100 - discountPercent) / 100, 0);
        }
    }
}
