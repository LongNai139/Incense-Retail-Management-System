namespace SV22T1080045.Shop.BusinessLayers.Helpers
{
    public static class ProductImageUrlHelper
    {
        public const string ProductImagesWebRoot = "/images/products";

        public static string? Normalize(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;

            if (imageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                imageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                imageUrl.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                return imageUrl;
            }

            return $"{ProductImagesWebRoot}/{ExtractFileName(imageUrl)}";
        }

        public static string? ExtractFileName(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;

            return Path.GetFileName(imageUrl.Replace('\\', '/'));
        }
    }
}
