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
        // ── Thông tin cơ bản ──────────────────────────────────────────────────
        public string ProductName { get; set; } = "";
        public string? Description { get; set; }       // Tab "Mô tả sản phẩm"

        // ── Thông số kỹ thuật (hiển thị trong specs-mini) ────────────────────
        public string? Ingredient { get; set; }        // Thành phần
        public string? BurningTime { get; set; }       // Thời gian cháy (vd: "45–60 phút")
        public string? Origin { get; set; }            // Xuất xứ (vd: "Bình Định, Việt Nam")
        public string? AgeYear { get; set; }           // Độ tuổi trầm (vd: "8–12 năm")
        public string? OilContent { get; set; }        // Hàm lượng tinh dầu (vd: "≥ 4,5%")
        public string? Length { get; set; }            // Chiều dài que / kích thước (vd: "22 cm")
        public string? Weight { get; set; }            // Trọng lượng (vd: "80g / hộp")

        // ── Tồn kho & bán hàng ───────────────────────────────────────────────
        public int? Quantity { get; set; }             // Số lượng tồn kho
        public int? SoldCount { get; set; }            // Số lượng đã bán

        // ── Đánh giá ─────────────────────────────────────────────────────────
        public int? Rating { get; set; }               // Đánh giá trung bình (0–5)
        public int? ReviewCount { get; set; }          // Số lượng đánh giá

        // ── Giá ──────────────────────────────────────────────────────────────
        public decimal ImportPrice { get; set; } = 0;  // Giá nhập
        public decimal SalePrice { get; set; } = 0;    // Giá bán trước giảm
        public decimal DiscountPercent { get; set; } = 0;
        public decimal OriginalPrice { get; set; } = 0;
        public decimal PriceAfterDiscount { get; set; } = 0;

        // ── Hình ảnh ─────────────────────────────────────────────────────────
        public string? ImageUrl { get; set; }          // Ảnh chính
        public string? ImageUrl2 { get; set; }         // Ảnh phụ 2
        public string? ImageUrl3 { get; set; }         // Ảnh phụ 3
        public string? ImageUrl4 { get; set; }         // Ảnh phụ 4

        // ── Usage Tags (phân tách bằng dấu phẩy, vd: "Thờ cúng,Thiền định,Thư giãn") ──
        public string? UsageTags { get; set; }

        // ── Khóa ngoại ───────────────────────────────────────────────────────
        public int CategoryId { get; set; }
        public int UnitId { get; set; }

        // ── Navigation properties ─────────────────────────────────────────────
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [ForeignKey("UnitId")]
        public virtual Unit? Unit { get; set; }

        public virtual ProductInventory? Inventory { get; set; }

        [NotMapped]
        public decimal DisplaySalePrice => SalePrice > 0 ? SalePrice : OriginalPrice;

        [NotMapped]
        public decimal DisplayPrice
        {
            get
            {
                if (PriceAfterDiscount > 0)
                    return PriceAfterDiscount;

                if (DisplaySalePrice <= 0)
                    return 0;

                return Math.Round(DisplaySalePrice * (100 - DiscountPercent) / 100, 0);
            }
        }

        [NotMapped]
        public int DisplayQuantity => Inventory?.Quantity ?? Quantity ?? 0;

        [NotMapped]
        public int DisplayLowStockThreshold => Inventory?.LowStockThreshold ?? 5;
    }
}
