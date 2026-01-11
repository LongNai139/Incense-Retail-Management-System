using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SV22T1080045.Shop.Models
{
    public class ProductEditModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        public string ProductName { get; set; } = "";

        public int UnitId { get; set; }
        public int CategoryId { get; set; }

        // Use Range to prevent negative numbers if needed
        public decimal OriginalPrice { get; set; }
        public decimal PriceAfterDiscount { get; set; }

        // Nullable strings (?) allow these fields to be empty in the database
        public string? BurningTime { get; set; }
        public string? Ingredient { get; set; }
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        // Critical: Captures the file uploaded from the Form.
        // Note: Domain Models cannot handle IFormFile types, hence the need for ViewModel.
        public IFormFile? UploadPhoto { get; set; }
    }
}