using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SV22T1080045.Shop.Models.ViewModels.Product
{
    public class ProductEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        public string ProductName { get; set; } = "";

        [Range(1, int.MaxValue, ErrorMessage = "Đơn vị tính không hợp lệ")]
        public int UnitId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Loại hàng không hợp lệ")]
        public int CategoryId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá nhập không hợp lệ")]
        public decimal ImportPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá bán không hợp lệ")]
        public decimal SalePrice { get; set; }

        [Range(0, 100, ErrorMessage = "Giảm giá phải từ 0 đến 100%")]
        public decimal DiscountPercent { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá gốc không hợp lệ")]
        public decimal OriginalPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá sau giảm không hợp lệ")]
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
        public string? ImageUrl { get; set; }
        public string? ImageUrl2 { get; set; }
        public string? ImageUrl3 { get; set; }
        public string? ImageUrl4 { get; set; }
        public IFormFile? UploadPhoto { get; set; }
        public IFormFile? UploadPhoto2 { get; set; }
        public IFormFile? UploadPhoto3 { get; set; }
        public IFormFile? UploadPhoto4 { get; set; }
    }
}
