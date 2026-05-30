using SV22T1080045.Shop.Abstractions.Models;

namespace SV22T1080045.Shop.Abstractions.Contracts.Products
{
    public class ProductSaveRequest
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = "";
        public int UnitId { get; set; }
        public int CategoryId { get; set; }

        public decimal ImportPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal PriceAfterDiscount { get; set; }

        public string? BurningTime { get; set; }
        public string? Ingredient { get; set; }
        public string? Description { get; set; }
        public string? Origin { get; set; }
        public string? AgeYear { get; set; }
        public string? OilContent { get; set; }
        public string? Length { get; set; }
        public string? Weight { get; set; }
        public string? UsageTags { get; set; }

        public int? Quantity { get; set; }
        public int? LowStockThreshold { get; set; }
        public int? SoldCount { get; set; }
        public int? Rating { get; set; }
        public int? ReviewCount { get; set; }

        public string? ExistingImageUrl { get; set; }
        public string? ExistingImageUrl2 { get; set; }
        public string? ExistingImageUrl3 { get; set; }
        public string? ExistingImageUrl4 { get; set; }

        public FileUploadData? UploadPhoto { get; set; }
        public FileUploadData? UploadPhoto2 { get; set; }
        public FileUploadData? UploadPhoto3 { get; set; }
        public FileUploadData? UploadPhoto4 { get; set; }
    }
}
