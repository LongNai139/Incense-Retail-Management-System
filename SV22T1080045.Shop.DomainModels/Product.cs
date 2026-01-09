using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.DomainModels
{
    [Table("Products")]
    public class Product : _BaseEntity
    {
        public string ProductName { get; set; } = "";
        public string? Description { get; set; }
        public string? Ingredient { get; set; } // Thành phần
        public string? BurningTime { get; set; } // Thời gian cháy

        public decimal OriginalPrice { get; set; } = 0;
        public decimal PriceAfterDiscount { get; set; } = 0;
        public string? ImageUrl { get; set; }

        // Khóa ngoại
        public int CategoryId { get; set; }
        public int UnitId { get; set; }
    }
}
